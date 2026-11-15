namespace ATM2.Core.Models
{
    /// <summary>
    /// Sesión en memoria (no se guarda en JSON). Expira tras
    /// AtmSettings.SessionTimeoutMinutes minutos de inactividad.
    /// </summary>
    public class Session
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
    }
}
