using ATM2.Core.Enums;

namespace ATM2.Core.Models
{
    public class Credit
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal RemainingBalance { get; set; }
        public float InterestRate { get; set; }
        public CreditStatus Status { get; set; } = CreditStatus.Pending;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
    }
}