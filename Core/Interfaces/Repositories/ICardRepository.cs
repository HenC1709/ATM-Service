using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Repositories
{
    public interface ICardRepository
    {
        Card? GetById(string id);
        Card? GetByNumber(string number);
        List<Card> GetByUserId(string userId);
        List<Card> GetAll();
        void Add(Card card);
        void Update(Card card);
    }
}
