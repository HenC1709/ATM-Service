using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class CreditRepository : JsonRepository<Credit>, ICreditRepository
    {
        public CreditRepository(JsonService jsonService)
            : base(jsonService, JsonContext.CreditsFile) { }

        public List<Credit> GetByUserId(string userId) =>
            GetAll().Where(c => c.UserId == userId).ToList();
    }
}
