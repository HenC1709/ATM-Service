using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly JsonService _jsonService;
        public UserRepository(JsonService jsonService)
        {
            _jsonService = jsonService;
        }

         public User? GetById(string id) =>
         GetAll().FirstOrDefault(a => a.Id == id);

        public User? GetByEmail(string email) =>
        GetAll().FirstOrDefault(u => u.Email == email);

        public List<User> GetAll() =>
         _jsonService.ReadALL<User>(JsonContext.UsersFile);

         public void Add(User user)
        {
            var users = GetAll();
            users.Add(user);
            _jsonService.WriteAll(JsonContext.UsersFile, users);
        }
         public void Update(User user)
        {
            var users = GetAll();
            int index = users.FindIndex(u => u.Id == user.Id);
            if (index != -1) users[index] = user;
            _jsonService.WriteAll(JsonContext.UsersFile, users);
        }
        public void Delete(string id)
        {
            var users = GetAll();
            users.RemoveAll(u => u.Id == id);
            _jsonService.WriteAll(JsonContext.UsersFile, users);
        }

    }
}