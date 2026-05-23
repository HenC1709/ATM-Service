using ATM2.Core.Enums;

namespace ATM2.Core.Models
{
    public class Account
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public AccountType Type { get; set; } = AccountType.Savings;
        public decimal Balance { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreateAdt { get; set; } = DateTime.UtcNow;
    }
}