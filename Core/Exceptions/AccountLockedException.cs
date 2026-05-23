using ATM2.Core.Models;

namespace ATM2.Core.Exceptions
{
    public class AccountLockedException : Exception
    {
        public AccountLockedException(string userId)
            : base($"Account for user '{userId}' is locked") { }
    }
}