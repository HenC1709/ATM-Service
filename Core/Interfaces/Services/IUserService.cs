using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface IUserService
    {
        User? GetById(string id);
        User? GetByEmail(string email);
        List<User> GetAll();
        void Update(User user);
        void Lock(string userId);
        void Unlock(string userId);
    }
}
