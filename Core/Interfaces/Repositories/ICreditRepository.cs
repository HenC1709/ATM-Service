using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Repositories
{
    public interface ICreditRepository
    {
        Credit? GetById(string id);
        List<Credit> GetByUserId(string userId);
        List<Credit> GetAll();
        void Add(Credit credit);
        void Update(Credit credit);
    }
}