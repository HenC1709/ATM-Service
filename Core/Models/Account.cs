using ATM2.Core.Enums;
using ATM2.Core.Interfaces;

namespace ATM2.Core.Models
{
    public class Account : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public AccountType Type { get; set; } = AccountType.Savings;
        public decimal Balance { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
