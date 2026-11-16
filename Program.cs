using System.Text;
using ATM2.Application;
using ATM2.Application.Services;
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Security;
using ATM2.Core.Interfaces.Services;
using ATM2.Infrastructure.Json;
using ATM2.Infrastructure.Repositories;
using ATM2.Infrastructure.Security;
using ATM2.Infrastructure.Seed;
using ATM2.UI;
using ATM2.UI.Menus;
using Microsoft.Extensions.DependencyInjection;

Console.OutputEncoding = Encoding.UTF8;
if (!Console.IsInputRedirected) Console.InputEncoding = Encoding.UTF8;

try
{
    var collection = new ServiceCollection();

    // --- Infraestructura (persistencia en JSON y seguridad) ---
    collection.AddSingleton(new JsonService(JsonContext.BasePath));
    collection.AddSingleton<IPasswordHasher, PasswordHasher>();
    collection.AddSingleton<IUserRepository, UserRepository>();
    collection.AddSingleton<IAccountRepository, AccountRepository>();
    collection.AddSingleton<ICardRepository, CardRepository>();
    collection.AddSingleton<ITransactionRepository, TransactionRepository>();
    collection.AddSingleton<ICreditRepository, CreditRepository>();
    collection.AddSingleton<ITicketRepository, TicketRepository>();

    // --- Servicios (lógica de negocio) ---
    collection.AddSingleton<ISessionService, SessionService>(); // debe ser singleton: guarda las sesiones en memoria
    collection.AddSingleton<ITransactionService, TransactionService>();
    collection.AddSingleton<IAccountService, AccountService>();
    collection.AddSingleton<IAuthService, AuthService>();
    collection.AddSingleton<IUserService, UserService>();
    collection.AddSingleton<ICardService, CardService>();
    collection.AddSingleton<ICreditService, CreditService>();
    collection.AddSingleton<ITicketService, TicketService>();

    // --- Interfaz de consola ---
    collection.AddSingleton<AppServices>();
    collection.AddSingleton<MainMenu>();

    using var provider = collection.BuildServiceProvider(new ServiceProviderOptions
    {
        ValidateOnBuild = true, // si falta registrar algo, falla al arrancar y no en mitad de una operación
        ValidateScopes = true
    });

    DataSeeder.Seed(provider.GetRequiredService<IUserRepository>(), provider.GetRequiredService<IPasswordHasher>());
    provider.GetRequiredService<MainMenu>().Run();
}
catch (Exception ex)
{
    ConsoleHelper.Error($"Error fatal: {ex.Message}");
    Environment.ExitCode = 1;
}
