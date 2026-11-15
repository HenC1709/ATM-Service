using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface IAuthService
    {
        /// <summary>Registra un usuario cliente y le crea su cuenta de ahorros.</summary>
        User Register(string fullName, string email, string password);

        /// <summary>Inicia sesión. Lanza InvalidCredentialsException o AccountLockedException.</summary>
        Session Login(string email, string password);

        void Logout(string sessionId);
        bool IsSessionActive(string sessionId);
    }
}
