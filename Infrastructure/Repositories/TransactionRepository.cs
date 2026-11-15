using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class TransactionRepository : JsonRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(JsonService jsonService)
            : base(jsonService, JsonContext.TransactionsFile) { }

        public List<Transaction> GetByAccountId(string accountId) =>
            GetAll().Where(t => t.AccountId == accountId).ToList();

        public List<Transaction> GetByUserId(string userId) =>
            GetAll().Where(t => t.UserId == userId).ToList();
    }
}
