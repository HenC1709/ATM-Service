using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class AccountRepository : JsonRepository<Account>, IAccountRepository
    {
        public AccountRepository(JsonService jsonService)
            : base(jsonService, JsonContext.AccountsFile) { }

        public Account? GetByUserId(string userId) =>
            GetAll().FirstOrDefault(a => a.UserId == userId);
    }
}
