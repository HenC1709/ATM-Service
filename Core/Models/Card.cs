using ATM2.Core.Enums;
using ATM2.Core.Interfaces;

namespace ATM2.Core.Models
{
    /// <summary>
    /// Tarjeta de débito simulada. El PIN pertenece a la tarjeta (como en un banco real)
    /// y se guarda hasheado, nunca en texto plano.
    /// </summary>
    public class Card : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string CardholderName { get; set; } = string.Empty;
        public string PinHash { get; set; } = string.Empty;
        public CardStatus Status { get; set; } = CardStatus.Active;
        public int FailedPinAttempts { get; set; } = 0;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
