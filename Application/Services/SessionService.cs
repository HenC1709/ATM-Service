using ATM2.Core;
using ATM2.Core.Exceptions;
using ATM2.Core.Interfaces.Services;
using ATM2.Core.Models;

namespace ATM2.Application.Services
{
    /// <summary>Sesiones en memoria: se pierden al cerrar la app (comportamiento esperado de un ATM).</summary>
    public class SessionService : ISessionService
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(AtmSettings.SessionTimeoutMinutes);
        private readonly Dictionary<string, Session> _sessions = new();

        public Session Create(string userId)
        {
            var session = new Session { UserId = userId };
            _sessions[session.Id] = session;
            return session;
        }

        public Session? GetById(string sessionId) => _sessions.GetValueOrDefault(sessionId);

        public Session? GetActive(string userId) =>
            _sessions.Values.FirstOrDefault(s => s.UserId == userId && IsActive(s.Id));

        public void End(string sessionId)
        {
            if (_sessions.TryGetValue(sessionId, out var session) && session.IsActive)
            {
                session.IsActive = false;
                session.EndedAt = DateTime.UtcNow;
            }
        }

        public bool IsActive(string sessionId)
        {
            if (!_sessions.TryGetValue(sessionId, out var session) || !session.IsActive)
                return false;

            if (DateTime.UtcNow - session.LastActivityAt > Timeout)
            {
                End(sessionId);
                return false;
            }
            return true;
        }

        public Session EnsureActive(string sessionId)
        {
            if (!IsActive(sessionId))
                throw new InvalidSessionException();

            var session = _sessions[sessionId];
            session.LastActivityAt = DateTime.UtcNow;
            return session;
        }
    }
}
