using System.Dynamic;
using ATM2.Core.Enums;

namespace ATM2.Core.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHas { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Client;
        public bool IsLocked { get; set; } = false;
        public DateTime CreateAdt { get; set; } = DateTime.UtcNow;

    }
}