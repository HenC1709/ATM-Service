using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface ISessionService
    {
        Session Create(string userId);
        Session? GetById(string sessionId);
        Session? GetActive(string userId);
        void End(string sessionId);
        bool IsActive(string sessionId);

        /// <summary>Verifica que la sesión siga activa y renueva su actividad. Lanza InvalidSessionException.</summary>
        Session EnsureActive(string sessionId);
    }
}
