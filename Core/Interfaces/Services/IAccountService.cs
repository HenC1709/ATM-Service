using ATM2.Core.Enums;
using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Account? GetById(string id);
        Account? GetByUserId(string userId);
        Account Create(string userId, AccountType type = AccountType.Savings);
        decimal GetBalance(string accountId);

        Transaction Deposit(string accountId, decimal amount, string? cardId = null,
            TransactionType type = TransactionType.Deposit, string? description = null);

        Transaction Withdraw(string accountId, decimal amount, string? cardId = null,
            TransactionType type = TransactionType.Withdrawal, string? description = null);

        void Transfer(string sourceAccountId, string targetAccountId, decimal amount, string? cardId = null);
    }
}
