using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly JsonService _jsonService;

        public AccountRepository(JsonService jsonService)
        {
            _jsonService = jsonService;
        }

        public Account? GetById(string id) =>
            GetAll().FirstOrDefault(a => a.Id == id);

        public Account? GetByUserId(string userId) =>
            GetAll().FirstOrDefault(a => a.UserId == userId);

        public List<Account> GetAll() =>
            _jsonService.ReadALL<Account>(JsonContext.AccountsFile);

        public void Add(Account account)
        {
            var accounts = GetAll();
            accounts.Add(account);
            _jsonService.WriteAll(JsonContext.AccountsFile, accounts);
        }

        public void Update(Account account)
        {
            var accounts = GetAll();
            int index = accounts.FindIndex(a => a.Id == account.Id);
            if (index != -1) accounts[index] = account;
            _jsonService.WriteAll(JsonContext.AccountsFile, accounts);
        }

        public void Delete(string id)
        {
            var accounts = GetAll();
            accounts.RemoveAll(a => a.Id == id);
            _jsonService.WriteAll(JsonContext.AccountsFile, accounts);
        }
    }
}