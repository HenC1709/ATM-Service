using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Services;
using ATM2.Core.Models;

namespace ATM2.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactions;

        public TransactionService(ITransactionRepository transactions)
        {
            _transactions = transactions;
        }

        public Transaction? GetById(string id) => _transactions.GetById(id);

        public List<Transaction> GetByAccountId(string accountId) =>
            _transactions.GetByAccountId(accountId).OrderByDescending(t => t.CreatedAt).ToList();

        public List<Transaction> GetByUserId(string userId) =>
            _transactions.GetByUserId(userId).OrderByDescending(t => t.CreatedAt).ToList();

        public List<Transaction> GetAll() =>
            _transactions.GetAll().OrderByDescending(t => t.CreatedAt).ToList();

        public void Register(Transaction transaction) => _transactions.Add(transaction);
    }
}
