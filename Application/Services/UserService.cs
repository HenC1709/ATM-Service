using ATM2.Core.Exceptions;
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Services;
using ATM2.Core.Models;

namespace ATM2.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;

        public UserService(IUserRepository users)
        {
            _users = users;
        }

        public User? GetById(string id) => _users.GetById(id);

        public User? GetByEmail(string email) => _users.GetByEmail(email);

        public List<User> GetAll() => _users.GetAll();

        public void Update(User user) => _users.Update(user);

        public void Lock(string userId)
        {
            var user = GetExisting(userId);
            user.IsLocked = true;
            _users.Update(user);
        }

        public void Unlock(string userId)
        {
            var user = GetExisting(userId);
            user.IsLocked = false;
            user.FailedLoginAttempts = 0;
            _users.Update(user);
        }

        private User GetExisting(string userId) =>
            _users.GetById(userId) ?? throw new UserNotFoundException(userId);
    }
}
