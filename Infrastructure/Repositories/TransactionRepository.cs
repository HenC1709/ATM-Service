using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly JsonService _jsonService;

        public TransactionRepository(JsonService jsonService)
        {
            _jsonService = jsonService;
        }

        public Transaction? GetById(string id) =>
            GetAll().FirstOrDefault(t => t.Id == id);

        public List<Transaction> GetByAccountId(string accountId) =>
            GetAll().Where(t => t.AccountId == accountId).ToList();

        public List<Transaction> GetAll() =>
            _jsonService.ReadALL<Transaction>(JsonContext.TransactionsFile);

        public void Add(Transaction transaction)
        {
            var transactions = GetAll();
            transactions.Add(transaction);
            _jsonService.WriteAll(JsonContext.TransactionsFile, transactions);
        }
    }
}