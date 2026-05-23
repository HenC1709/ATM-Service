using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface ISessionService
    {
        Session? GetActive(string userId);
        Session Create(string userId);
        void End(string sessionId);
        bool IsActive(string sessionId);
    }
}