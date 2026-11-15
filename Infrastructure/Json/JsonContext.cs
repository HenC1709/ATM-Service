namespace ATM2.Infrastructure.Json
{
    public static class JsonContext
    {
        /// <summary>
        /// Carpeta donde se guardan los .json. Se usa el directorio de trabajo
        /// (la raíz del proyecto al ejecutar "dotnet run" o desde VS Code),
        /// así los datos no se pierden al limpiar bin/.
        /// </summary>
        public static readonly string BasePath = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        public const string UsersFile = "users.json";
        public const string AccountsFile = "accounts.json";
        public const string CardsFile = "cards.json";
        public const string TransactionsFile = "transactions.json";
        public const string CreditsFile = "credits.json";
        public const string TicketsFile = "tickets.json";
    }
}
