using ATM2.Application;
using ATM2.Core;
using ATM2.Core.Enums;
using ATM2.Core.Exceptions;
using ATM2.Core.Models;

namespace ATM2.UI.Menus
{
    /// <summary>Menú del cliente después de iniciar sesión ("banca en línea").</summary>
    public class UserMenu : SessionMenu
    {
        public UserMenu(AppServices services, Session session, User user) : base(services, session, user) { }

        protected override string Title => $"BIENVENIDO(A), {User.FullName.ToUpperInvariant()}";

        protected override string[] Options => new[]
        {
            "Ver mi cuenta",
            "Solicitar tarjeta",
            "Mis tarjetas",
            "Usar cajero (insertar tarjeta)",
            "Solicitar crédito",
            "Mis créditos y deudas",
            "Historial de movimientos",
            "Crear ticket de soporte",
            "Mis tickets de soporte"
        };

        protected override void BeforeMenu()
        {
            var account = Services.Accounts.GetByUserId(User.Id);
            if (account is not null)
                ConsoleHelper.Info($"Saldo disponible: {Display.Money(account.Balance)}\n");
        }

        protected override void Handle(int option)
        {
            switch (option)
            {
                case 1: ShowAccount(); break;
                case 2: RequestCard(); break;
                case 3: ShowCards(); break;
                case 4: UseAtm(); break;
                case 5: RequestCredit(); break;
                case 6: ShowCredits(); break;
                case 7: ShowHistory(); break;
                case 8: CreateTicket(); break;
                case 9: ShowTickets(); break;
            }
        }

        private Account GetAccount() =>
            Services.Accounts.GetByUserId(User.Id) ?? throw new AtmException("No tienes una cuenta asociada.");

        private void ShowAccount()
        {
            var account = GetAccount();
            var cards = Services.Cards.GetByUserId(User.Id);
            decimal debt = Services.Credits.GetPendingDebts(User.Id).Sum(c => c.RemainingBalance);

            ConsoleHelper.Section("MI CUENTA");
            Console.WriteLine($"Titular:          {User.FullName}");
            Console.WriteLine($"Correo:           {User.Email}");
            Console.WriteLine($"Tipo de cuenta:   {Display.Name(account.Type)}");
            Console.WriteLine($"N.º de cuenta:    {Display.ShortId(account.Id)}");
            Console.WriteLine($"Saldo:            {Display.Money(account.Balance)}");
            Console.WriteLine($"Tarjetas activas: {cards.Count(c => c.Status == CardStatus.Active)}");
            Console.WriteLine($"Deuda pendiente:  {Display.Money(debt)}");
        }

        private void RequestCard()
        {
            ConsoleHelper.Section("SOLICITAR TARJETA");
            ConsoleHelper.Info($"Elige un PIN de {AtmSettings.PinLength} dígitos para tu nueva tarjeta.");
            string pin = ConsoleHelper.ReadSecret("PIN");
            string confirm = ConsoleHelper.ReadSecret("Confirmar PIN");
            if (pin != confirm)
            {
                ConsoleHelper.Error("Los PIN no coinciden.");
                return;
            }

            ConsoleHelper.Info("Procesando solicitud...");
            var card = Services.Cards.Request(User.Id, pin);

            Console.WriteLine();
            ConsoleHelper.Success("¡Tarjeta emitida! (simulada)");
            Console.WriteLine($"  Número:  {Display.FormatCardNumber(card.Number)}");
            Console.WriteLine($"  Titular: {card.CardholderName}");
            Console.WriteLine($"  Vence:   {card.ExpiresAt.ToLocalTime():MM/yy}");
        }

        private void ShowCards()
        {
            ConsoleHelper.Section("MIS TARJETAS");
            var cards = Services.Cards.GetByUserId(User.Id);
            if (cards.Count == 0)
            {
                ConsoleHelper.Info("Aún no tienes tarjetas. Usa la opción 'Solicitar tarjeta'.");
                return;
            }
            foreach (var card in cards)
                Console.WriteLine($"  {Display.MaskCard(card.Number)}  Vence: {card.ExpiresAt.ToLocalTime():MM/yy}  Estado: {Display.Name(card.Status)}");
        }

        private void UseAtm()
        {
            ConsoleHelper.Section("INSERTAR TARJETA");
            var cards = Services.Cards.GetByUserId(User.Id)
                .Where(c => c.Status == CardStatus.Active)
                .ToList();

            if (cards.Count == 0)
            {
                ConsoleHelper.Info("No tienes tarjetas activas. Solicita una para usar el cajero.");
                return;
            }

            var card = ConsoleHelper.SelectFromList(cards, c => Display.MaskCard(c.Number), "Tarjeta a insertar");
            if (card is null) return;

            // Pedir PIN hasta acertar o hasta que la tarjeta se bloquee (CardBlockedException sale al menú).
            while (true)
            {
                string pin = ConsoleHelper.ReadSecret("Ingresa tu PIN");
                try
                {
                    card = Services.Cards.ValidatePin(card.Id, pin);
                    break;
                }
                catch (InvalidPinException ex)
                {
                    ConsoleHelper.Warning(ex.Message);
                }
            }

            new AtmMenu(Services, Session, User, card).Run();
            ConsoleHelper.Info("Volviste al menú principal.");
        }

        private void RequestCredit()
        {
            ConsoleHelper.Section("SOLICITAR CRÉDITO");
            ConsoleHelper.Info($"Interés: {AtmSettings.CreditInterestRate:P0}. Máximo: {Display.Money(AtmSettings.MaxCreditAmount)}.");
            ConsoleHelper.Info($"Hasta {Display.Money(AtmSettings.CreditAutoApprovalLimit)} se aprueba al instante; montos mayores los revisa un administrador.");
            Console.WriteLine();

            decimal? amount = ConsoleHelper.ReadAmount("Monto solicitado");
            if (amount is null) { ConsoleHelper.Cancelled(); return; }

            decimal total = Math.Round(amount.Value * (1 + AtmSettings.CreditInterestRate), 2);
            if (!ConsoleHelper.Confirm($"Recibirás {Display.Money(amount.Value)} y deberás pagar {Display.Money(total)}. ¿Continuar?"))
            {
                ConsoleHelper.Cancelled();
                return;
            }

            var credit = Services.Credits.Request(User.Id, amount.Value);
            if (credit.Status == CreditStatus.Approved)
                ConsoleHelper.Success($"Crédito aprobado. Se depositaron {Display.Money(credit.Amount)} en tu cuenta.");
            else
                ConsoleHelper.Warning("Tu solicitud quedó pendiente de aprobación por un administrador.");
        }

        private void ShowCredits()
        {
            ConsoleHelper.Section("MIS CRÉDITOS Y DEUDAS");
            var credits = Services.Credits.GetByUserId(User.Id);
            if (credits.Count == 0)
            {
                ConsoleHelper.Info("No tienes créditos.");
                return;
            }
            foreach (var c in credits)
                Console.WriteLine($"  [{Display.ShortId(c.Id)}] {Display.Date(c.RequestedAt)}  Monto: {Display.Money(c.Amount)}  " +
                                  $"Total: {Display.Money(c.TotalToPay)}  Pendiente: {Display.Money(c.RemainingBalance)}  {Display.Name(c.Status)}");
            Console.WriteLine();
            ConsoleHelper.Info("Las deudas se pagan desde el cajero (opción 'Usar cajero' > 'Pagar deuda').");
        }

        private void ShowHistory()
        {
            ConsoleHelper.Section($"HISTORIAL DE MOVIMIENTOS (últimos {AtmSettings.HistoryRows})");
            var account = GetAccount();
            Display.PrintTransactions(Services.Transactions.GetByAccountId(account.Id).Take(AtmSettings.HistoryRows));
        }

        private void CreateTicket()
        {
            ConsoleHelper.Section("NUEVO TICKET DE SOPORTE");
            string subject = ConsoleHelper.ReadText("Asunto");
            string description = ConsoleHelper.ReadText("Describe el problema");
            var ticket = Services.Tickets.Create(User.Id, subject, description);
            ConsoleHelper.Success($"Ticket #{Display.ShortId(ticket.Id)} creado. Un administrador lo revisará.");
        }

        private void ShowTickets()
        {
            ConsoleHelper.Section("MIS TICKETS");
            var tickets = Services.Tickets.GetByUserId(User.Id);
            if (tickets.Count == 0)
            {
                ConsoleHelper.Info("No tienes tickets.");
                return;
            }
            foreach (var t in tickets)
                Console.WriteLine($"  #{Display.ShortId(t.Id)}  {Display.Date(t.CreatedAt)}  [{Display.Name(t.Status)}]  {t.Subject}");
        }
    }
}
