using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class CardRepository : JsonRepository<Card>, ICardRepository
    {
        public CardRepository(JsonService jsonService)
            : base(jsonService, JsonContext.CardsFile) { }

        public Card? GetByNumber(string number) =>
            GetAll().FirstOrDefault(c => c.Number == number);

        public List<Card> GetByUserId(string userId) =>
            GetAll().Where(c => c.UserId == userId).ToList();
    }
}
