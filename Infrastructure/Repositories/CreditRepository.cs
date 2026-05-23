using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class CreditRepository : ICreditRepository
    {
        private readonly JsonService _jsonService;

        public CreditRepository(JsonService jsonService)
        {
            _jsonService = jsonService;
        }

        public Credit? GetById(string id) =>
            GetAll().FirstOrDefault(c => c.Id == id);

        public List<Credit> GetByUserId(string userId) =>
            GetAll().Where(c => c.UserId == userId).ToList();

        public List<Credit> GetAll() =>
            _jsonService.ReadALL<Credit>(JsonContext.CreditsFile);

        public void Add(Credit credit)
        {
            var credits = GetAll();
            credits.Add(credit);
            _jsonService.WriteAll(JsonContext.CreditsFile, credits);
        }

        public void Update(Credit credit)
        {
            var credits = GetAll();
            int index = credits.FindIndex(c => c.Id == credit.Id);
            if (index != -1) credits[index] = credit;
            _jsonService.WriteAll(JsonContext.CreditsFile, credits);
        }
    }
}