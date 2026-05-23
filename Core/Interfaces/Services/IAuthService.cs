using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Session? Login(string email, string password);
        void Logout(string sessionId);
        void ValidatePin(string userId, string pin);
        void IsSessionActive(string sessionId);
    }
}