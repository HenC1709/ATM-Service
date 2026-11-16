using ATM2.Application;
using ATM2.Core;
using ATM2.Core.Exceptions;
using ATM2.Core.Models;

namespace ATM2.UI.Menus
{
    /// <summary>Operaciones del cajero con una tarjeta insertada y PIN validado.</summary>
    public class AtmMenu : SessionMenu
    {
        private static readonly decimal[] QuickAmounts = AtmSettings.QuickWithdrawalAmounts;
        private readonly Card _card;

        public AtmMenu(AppServices services, Session session, User user, Card card) : base(services, session, user)
        {
            _card = card;
        }

        protected override string Title => $"CAJERO - {Display.MaskCard(_card.Number)}";
        protected override string ExitLabel => "Retirar tarjeta";
        protected override bool IsNested => true;

        protected override string[] Options => new[]
        {
            "Consultar saldo",
            "Retirar dinero",
            "Depositar dinero",
            "Pagar deuda",
            "Transferir a otro usuario",
            "Últimos movimientos",
            "Cambiar PIN"
        };

        // Si la tarjeta se bloquea (p. ej. fallando el PIN al cambiarlo) se expulsa.
        protected override bool ShouldExitOn(AtmException ex) => ex is CardBlockedException;

        protected override void OnExit() => ConsoleHelper.Success("Retire su tarjeta. ¡Gracias por usar ATM2!");

        protected override void Handle(int option)
        {
            switch (option)
            {
                case 1: ShowBalance(); break;
                case 2: Withdraw(); break;
                case 3: Deposit(); break;
                case 4: PayDebt(); break;
                case 5: Transfer(); break;
                case 6: ShowMovements(); break;
                case 7: ChangePin(); break;
            }
        }

        private void ShowBalance()
        {
            ConsoleHelper.Section("CONSULTA DE SALDO");
            Console.WriteLine($"Saldo disponible: {Display.Money(Services.Accounts.GetBalance(_card.AccountId))}");
        }

        private void Withdraw()
        {
            ConsoleHelper.Section("RETIRO DE EFECTIVO");
            var options = QuickAmounts.Select(Display.Money).Append("Otro monto").ToArray();
            int choice = ConsoleHelper.ShowMenu(options, "Cancelar");
            if (choice == 0) { ConsoleHelper.Cancelled(); return; }

            decimal amount;
            if (choice <= QuickAmounts.Length)
            {
                amount = QuickAmounts[choice - 1];
            }
            else
            {
                ConsoleHelper.Info($"Múltiplos de {Display.Money(AtmSettings.WithdrawalMultiple)}, máximo {Display.Money(AtmSettings.MaxWithdrawalPerTransaction)}.");
                decimal? custom = ConsoleHelper.ReadAmount("Monto a retirar");
                if (custom is null) { ConsoleHelper.Cancelled(); return; }
                amount = custom.Value;
            }

            if (!ConsoleHelper.Confirm($"¿Retirar {Display.Money(amount)}?")) { ConsoleHelper.Cancelled(); return; }

            var transaction = Services.Accounts.Withdraw(_card.AccountId, amount, _card.Id);
            ConsoleHelper.Success("Retire su dinero.");
            Display.PrintReceipt(transaction, _card);
        }

        private void Deposit()
        {
            ConsoleHelper.Section("DEPÓSITO");
            decimal? amount = ConsoleHelper.ReadAmount("Monto a depositar");
            if (amount is null) { ConsoleHelper.Cancelled(); return; }

            ConsoleHelper.Info("Introduzca los billetes en la ranura... (simulado)");
            var transaction = Services.Accounts.Deposit(_card.AccountId, amount.Value, _card.Id);
            ConsoleHelper.Success("Depósito realizado.");
            Display.PrintReceipt(transaction, _card);
        }

        private void PayDebt()
        {
            ConsoleHelper.Section("PAGAR DEUDA");
            var debts = Services.Credits.GetPendingDebts(User.Id);
            if (debts.Count == 0)
            {
                ConsoleHelper.Success("No tienes deudas pendientes.");
                return;
            }

            var credit = ConsoleHelper.SelectFromList(debts,
                c => $"Crédito {Display.ShortId(c.Id)} - Pendiente: {Display.Money(c.RemainingBalance)}",
                "Deuda a pagar");
            if (credit is null) { ConsoleHelper.Cancelled(); return; }

            Console.WriteLine();
            int mode = ConsoleHelper.ShowMenu(new[] { $"Pagar total ({Display.Money(credit.RemainingBalance)})", "Pago parcial" }, "Cancelar");
            if (mode == 0) { ConsoleHelper.Cancelled(); return; }

            decimal amount = credit.RemainingBalance;
            if (mode == 2)
            {
                decimal? partial = ConsoleHelper.ReadAmount("Monto a pagar");
                if (partial is null) { ConsoleHelper.Cancelled(); return; }
                amount = partial.Value;
            }

            if (!ConsoleHelper.Confirm($"¿Pagar {Display.Money(amount)}?")) { ConsoleHelper.Cancelled(); return; }

            var transaction = Services.Credits.Pay(credit.Id, amount, _card.Id);
            var updated = Services.Credits.GetById(credit.Id)!;
            ConsoleHelper.Success(updated.RemainingBalance == 0
                ? "¡Deuda pagada por completo!"
                : $"Pago registrado. Deuda restante: {Display.Money(updated.RemainingBalance)}");
            Display.PrintReceipt(transaction, _card);
        }

        private void Transfer()
        {
            ConsoleHelper.Section("TRANSFERENCIA");
            string email = ConsoleHelper.ReadText("Correo del destinatario");
            var recipient = Services.Users.GetByEmail(email) ?? throw new UserNotFoundException(email);
            if (recipient.Id == User.Id) throw new AtmException("No puedes transferirte a ti mismo.");

            var targetAccount = Services.Accounts.GetByUserId(recipient.Id)
                ?? throw new AtmException("El destinatario no tiene una cuenta.");

            decimal? amount = ConsoleHelper.ReadAmount("Monto a transferir");
            if (amount is null) { ConsoleHelper.Cancelled(); return; }

            if (!ConsoleHelper.Confirm($"¿Transferir {Display.Money(amount.Value)} a {recipient.FullName}?"))
            {
                ConsoleHelper.Cancelled();
                return;
            }

            Services.Accounts.Transfer(_card.AccountId, targetAccount.Id, amount.Value, _card.Id);
            ConsoleHelper.Success($"Transferencia enviada a {recipient.FullName}.");
            Console.WriteLine($"Nuevo saldo: {Display.Money(Services.Accounts.GetBalance(_card.AccountId))}");
        }

        private void ShowMovements()
        {
            ConsoleHelper.Section($"ÚLTIMOS {AtmSettings.MiniStatementRows} MOVIMIENTOS");
            Display.PrintTransactions(Services.Transactions.GetByAccountId(_card.AccountId).Take(AtmSettings.MiniStatementRows));
        }

        private void ChangePin()
        {
            ConsoleHelper.Section("CAMBIAR PIN");
            string current = ConsoleHelper.ReadSecret("PIN actual");
            string newPin = ConsoleHelper.ReadSecret("Nuevo PIN");
            string confirm = ConsoleHelper.ReadSecret("Confirmar nuevo PIN");
            if (newPin != confirm)
            {
                ConsoleHelper.Error("Los PIN nuevos no coinciden.");
                return;
            }

            Services.Cards.ChangePin(_card.Id, current, newPin);
            ConsoleHelper.Success("PIN actualizado correctamente.");
        }
    }
}
