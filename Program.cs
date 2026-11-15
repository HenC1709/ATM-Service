using System.Text;
using ATM2.Application;
using ATM2.Application.Services;
using ATM2.Infrastructure.Json;
using ATM2.Infrastructure.Repositories;
using ATM2.Infrastructure.Security;
using ATM2.Infrastructure.Seed;
using ATM2.UI;
using ATM2.UI.Menus;

Console.OutputEncoding = Encoding.UTF8;
if (!Console.IsInputRedirected) Console.InputEncoding = Encoding.UTF8;

try
{
    // --- Infraestructura (persistencia en JSON) ---
    var jsonService = new JsonService(JsonContext.BasePath);
    var userRepository = new UserRepository(jsonService);
    var accountRepository = new AccountRepository(jsonService);
    var cardRepository = new CardRepository(jsonService);
    var transactionRepository = new TransactionRepository(jsonService);
    var creditRepository = new CreditRepository(jsonService);
    var ticketRepository = new TicketRepository(jsonService);
    var hasher = new PasswordHasher();

    // --- Servicios (lógica de negocio) ---
    var sessionService = new SessionService();
    var transactionService = new TransactionService(transactionRepository);
    var accountService = new AccountService(accountRepository, transactionService);
    var services = new AppServices(
        Auth: new AuthService(userRepository, accountService, sessionService, hasher),
        Sessions: sessionService,
        Users: new UserService(userRepository),
        Accounts: accountService,
        Cards: new CardService(cardRepository, userRepository, accountRepository, hasher),
        Transactions: transactionService,
        Credits: new CreditService(creditRepository, accountService),
        Tickets: new TicketService(ticketRepository));

    DataSeeder.Seed(userRepository, hasher);

    // --- Interfaz de consola ---
    new MainMenu(services).Run();
}
catch (Exception ex)
{
    ConsoleHelper.Error($"Error fatal: {ex.Message}");
    Environment.ExitCode = 1;
}
