using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface ITransactionService
    {
        Transaction? GetById(string id);
        List<Transaction> GetByAccountId(string accountId);
        List<Transaction> GetByUserId(string userId);
        List<Transaction> GetAll();
        void Register(Transaction transaction);
    }
}
