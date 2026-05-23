using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Account? GetById(string id);
        Account? GetByUserId(string userId);
        List<Account> GetAll();
        void Add(Account account);
        void Update(Account account);
        void Delete(string id);
    }
}