using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class UserRepository : JsonRepository<User>, IUserRepository
    {
        public UserRepository(JsonService jsonService)
            : base(jsonService, JsonContext.UsersFile) { }

        public User? GetByEmail(string email) =>
            GetAll().FirstOrDefault(u => string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
