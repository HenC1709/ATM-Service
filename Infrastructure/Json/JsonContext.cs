namespace ATM2.Infrastructure.Json
{
    public class JsonContext
    {
        public static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure", "Data");

        public static readonly string UsersFile = "users.json";
        public static readonly string AccountsFile = "accounts.json";
        public static readonly string TransactionsFile = "transactions.json";
        public static readonly string CreditsFile = "credits.json";
        public static readonly string TicketsFile = "tickets.json";
    }
}