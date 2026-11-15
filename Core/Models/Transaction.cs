using ATM2.Core.Enums;
using ATM2.Core.Interfaces;

namespace ATM2.Core.Models
{
    /// <summary>
    /// Movimiento de una cuenta. Guarda UserId y CardId para saber
    /// quién hizo la operación y con qué tarjeta (auditoría).
    /// </summary>
    public class Transaction : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AccountId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? CardId { get; set; }
        public string? RelatedAccountId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
