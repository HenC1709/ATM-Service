using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Account? GetById(string id);
        Account? GetByUserId(string userId);
        void Create(Account account);
        void Deposit(string accountId, decimal amount);
        void Withdraw(string accountId, decimal amount);
        void Transfer(string sourceAccountId, string targetAccountId, decimal amount);
    }
}