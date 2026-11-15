using ATM2.Core.Interfaces.Services;

namespace ATM2.Application
{
    /// <summary>
    /// Agrupa todos los servicios para pasarlos a los menús sin constructores gigantes.
    /// Se arma una sola vez en Program.cs (inyección de dependencias manual).
    /// </summary>
    public sealed record AppServices(
        IAuthService Auth,
        ISessionService Sessions,
        IUserService Users,
        IAccountService Accounts,
        ICardService Cards,
        ITransactionService Transactions,
        ICreditService Credits,
        ITicketService Tickets);
}
