using ATM2.Application;
using ATM2.Core.Enums;
using ATM2.Core.Models;

namespace ATM2.UI.Menus
{
    /// <summary>Panel de administración: auditoría de movimientos y gestión de usuarios, tarjetas, créditos y tickets.</summary>
    public class AdminMenu : SessionMenu
    {
        public AdminMenu(AppServices services, Session session, User user) : base(services, session, user) { }

        protected override string Title => "PANEL DE ADMINISTRACIÓN";

        protected override string[] Options => new[]
        {
            "Ver usuarios",
            "Bloquear / desbloquear usuario",
            "Auditoría de transacciones",
            "Créditos pendientes de aprobación",
            "Gestionar tarjetas",
            "Tickets de soporte"
        };

        protected override void Handle(int option)
        {
            switch (option)
            {
                case 1: ShowUsers(); break;
                case 2: ToggleUserLock(); break;
                case 3: AuditTransactions(); break;
                case 4: ReviewCredits(); break;
                case 5: ManageCards(); break;
                case 6: ManageTickets(); break;
            }
        }

        private string UserName(string userId) =>
            Services.Users.GetById(userId)?.FullName ?? "(desconocido)";

        private void ShowUsers()
        {
            ConsoleHelper.Section("USUARIOS");
            foreach (var u in Services.Users.GetAll())
            {
                var account = Services.Accounts.GetByUserId(u.Id);
                string balance = account is null ? "-" : Display.Money(account.Balance);
                string status = u.IsLocked ? "BLOQUEADO" : "Activo";
                Console.WriteLine($"  {u.FullName,-22} {u.Email,-26} {u.Role,-7} {status,-10} Saldo: {balance}");
            }
        }

        private void ToggleUserLock()
        {
            ConsoleHelper.Section("BLOQUEAR / DESBLOQUEAR USUARIO");
            var clients = Services.Users.GetAll().Where(u => u.Role == UserRole.Client).ToList();
            if (clients.Count == 0) { ConsoleHelper.Info("No hay clientes registrados."); return; }

            var user = ConsoleHelper.SelectFromList(clients,
                u => $"{u.FullName} ({u.Email}) - {(u.IsLocked ? "BLOQUEADO" : "Activo")}", "Usuario");
            if (user is null) return;

            if (user.IsLocked)
            {
                Services.Users.Unlock(user.Id);
                ConsoleHelper.Success($"{user.FullName} fue desbloqueado.");
            }
            else
            {
                Services.Users.Lock(user.Id);
                ConsoleHelper.Success($"{user.FullName} fue bloqueado.");
            }
        }

        private void AuditTransactions()
        {
            ConsoleHelper.Section("AUDITORÍA DE TRANSACCIONES");
            int filter = ConsoleHelper.ShowMenu(new[] { "Todas", "Solo retiros", "Solo pagos de deuda", "Solo depósitos" });
            if (filter == 0) return;

            var transactions = Services.Transactions.GetAll().AsEnumerable();
            transactions = filter switch
            {
                2 => transactions.Where(t => t.Type == TransactionType.Withdrawal),
                3 => transactions.Where(t => t.Type == TransactionType.DebtPayment),
                4 => transactions.Where(t => t.Type == TransactionType.Deposit),
                _ => transactions
            };

            var cards = Services.Cards.GetAll().ToDictionary(c => c.Id);
            Console.WriteLine();
            Display.PrintTransactions(transactions.Take(50), t =>
            {
                string card = t.CardId is not null && cards.TryGetValue(t.CardId, out var c) ? Display.MaskCard(c.Number) : "sin tarjeta";
                return $"| {UserName(t.UserId)} ({card})";
            });
        }

        private void ReviewCredits()
        {
            ConsoleHelper.Section("CRÉDITOS PENDIENTES");
            var pending = Services.Credits.GetAll().Where(c => c.Status == CreditStatus.Pending).ToList();
            if (pending.Count == 0) { ConsoleHelper.Info("No hay créditos pendientes."); return; }

            var credit = ConsoleHelper.SelectFromList(pending,
                c => $"{UserName(c.UserId)} - {Display.Money(c.Amount)} ({Display.Date(c.RequestedAt)})", "Crédito");
            if (credit is null) return;

            int decision = ConsoleHelper.ShowMenu(new[] { "Aprobar", "Rechazar" }, "Cancelar");
            if (decision == 1)
            {
                Services.Credits.Approve(credit.Id);
                ConsoleHelper.Success("Crédito aprobado y depositado en la cuenta del cliente.");
            }
            else if (decision == 2)
            {
                Services.Credits.Reject(credit.Id);
                ConsoleHelper.Success("Crédito rechazado.");
            }
        }

        private void ManageCards()
        {
            ConsoleHelper.Section("TARJETAS");
            var cards = Services.Cards.GetAll().Where(c => c.Status != CardStatus.Cancelled).ToList();
            if (cards.Count == 0) { ConsoleHelper.Info("No hay tarjetas emitidas."); return; }

            var card = ConsoleHelper.SelectFromList(cards,
                c => $"{Display.MaskCard(c.Number)} - {UserName(c.UserId)} - {Display.Name(c.Status)}", "Tarjeta");
            if (card is null) return;

            if (card.Status == CardStatus.Blocked)
            {
                Services.Cards.Unblock(card.Id);
                ConsoleHelper.Success("Tarjeta desbloqueada.");
            }
            else
            {
                Services.Cards.Block(card.Id);
                ConsoleHelper.Success("Tarjeta bloqueada.");
            }
        }

        private void ManageTickets()
        {
            ConsoleHelper.Section("TICKETS DE SOPORTE");
            var tickets = Services.Tickets.GetAll();
            if (tickets.Count == 0) { ConsoleHelper.Info("No hay tickets."); return; }

            var ticket = ConsoleHelper.SelectFromList(tickets,
                t => $"#{Display.ShortId(t.Id)} [{Display.Name(t.Status)}] {UserName(t.UserId)}: {t.Subject}", "Ticket");
            if (ticket is null) return;

            Console.WriteLine();
            Console.WriteLine($"Descripción: {ticket.Description}");
            Console.WriteLine();

            var statuses = Enum.GetValues<TicketStatus>();
            int choice = ConsoleHelper.ShowMenu(statuses.Select(Display.Name).ToArray(), "Cancelar");
            if (choice == 0) return;

            Services.Tickets.UpdateStatus(ticket.Id, statuses[choice - 1]);
            ConsoleHelper.Success("Estado del ticket actualizado.");
        }
    }
}
