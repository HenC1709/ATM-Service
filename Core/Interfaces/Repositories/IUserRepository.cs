using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        User? GetById(string id);
        User? GetByEmail(string email);
        List<User> GettAll();
        void Add(User user);
        void Update(User user);
        void Delete(string id);
    }
}