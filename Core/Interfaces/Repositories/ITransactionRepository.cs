using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Transaction? GetById(string id);
        List<Transaction> GetByAccountId(string accountId);
        List<Transaction> GetByUserId(string userId);
        List<Transaction> GetAll();
        void Add(Transaction transaction);
    }
}
