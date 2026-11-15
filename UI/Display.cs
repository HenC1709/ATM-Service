using System.Globalization;
using ATM2.Core.Enums;
using ATM2.Core.Models;

namespace ATM2.UI
{
    /// <summary>Formatos y traducciones al español para mostrar datos en pantalla.</summary>
    public static class Display
    {
        public static string Money(decimal amount) => amount.ToString("C", CultureInfo.CurrentCulture);

        public static string Date(DateTime utc) => utc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        public static string MaskCard(string number) =>
            number.Length >= 4 ? $"**** **** **** {number[^4..]}" : number;

        public static string FormatCardNumber(string number) =>
            string.Join(' ', Enumerable.Range(0, number.Length / 4).Select(i => number.Substring(i * 4, 4)));

        public static string ShortId(string id) => id[..8].ToUpperInvariant();

        public static string Name(TransactionType type) => type switch
        {
            TransactionType.Deposit => "Depósito",
            TransactionType.Withdrawal => "Retiro",
            TransactionType.TransferOut => "Transferencia enviada",
            TransactionType.TransferIn => "Transferencia recibida",
            TransactionType.DebtPayment => "Pago de deuda",
            TransactionType.CreditDisbursement => "Desembolso de crédito",
            _ => type.ToString()
        };

        /// <summary>true si el movimiento suma dinero a la cuenta.</summary>
        public static bool IsIncome(TransactionType type) =>
            type is TransactionType.Deposit or TransactionType.TransferIn or TransactionType.CreditDisbursement;

        public static string Name(CardStatus status) => status switch
        {
            CardStatus.Active => "Activa",
            CardStatus.Blocked => "Bloqueada",
            CardStatus.Cancelled => "Cancelada",
            _ => status.ToString()
        };

        public static string Name(CreditStatus status) => status switch
        {
            CreditStatus.Pending => "Pendiente",
            CreditStatus.Approved => "Aprobado",
            CreditStatus.Rejected => "Rechazado",
            CreditStatus.Paid => "Pagado",
            _ => status.ToString()
        };

        public static string Name(TicketStatus status) => status switch
        {
            TicketStatus.Open => "Abierto",
            TicketStatus.InProgress => "En progreso",
            TicketStatus.Resolved => "Resuelto",
            TicketStatus.Closed => "Cerrado",
            _ => status.ToString()
        };

        public static string Name(AccountType type) => type switch
        {
            AccountType.Savings => "Ahorros",
            AccountType.Checking => "Corriente",
            _ => type.ToString()
        };

        public static void PrintTransactions(IEnumerable<Transaction> transactions, Func<Transaction, string>? extra = null)
        {
            var list = transactions.ToList();
            if (list.Count == 0)
            {
                ConsoleHelper.Info("No hay movimientos.");
                return;
            }

            foreach (var t in list)
            {
                string sign = IsIncome(t.Type) ? "+" : "-";
                Console.ForegroundColor = IsIncome(t.Type) ? ConsoleColor.Green : ConsoleColor.Yellow;
                Console.Write($"{Date(t.CreatedAt)}  {sign}{Money(t.Amount),14}  ");
                Console.ResetColor();
                Console.Write($"{Name(t.Type),-22} Saldo: {Money(t.BalanceAfter)}");
                if (extra is not null) Console.Write($"  {extra(t)}");
                Console.WriteLine();
            }
        }

        public static void PrintReceipt(Transaction transaction, Card card)
        {
            Console.WriteLine();
            Console.WriteLine("  ┌──────────────── COMPROBANTE ────────────────┐");
            Console.WriteLine($"  │ Fecha:     {Date(transaction.CreatedAt),-33}│");
            Console.WriteLine($"  │ Tarjeta:   {MaskCard(card.Number),-33}│");
            Console.WriteLine($"  │ Operación: {Name(transaction.Type),-33}│");
            Console.WriteLine($"  │ Monto:     {Money(transaction.Amount),-33}│");
            Console.WriteLine($"  │ Saldo:     {Money(transaction.BalanceAfter),-33}│");
            Console.WriteLine($"  │ Ref:       {ShortId(transaction.Id),-33}│");
            Console.WriteLine("  └─────────────────────────────────────────────┘");
        }
    }
}
