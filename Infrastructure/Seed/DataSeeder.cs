using ATM2.Core.Enums;
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Security;
using ATM2.Core.Models;

namespace ATM2.Infrastructure.Seed
{
    /// <summary>
    /// Crea un administrador por defecto la primera vez que se ejecuta la app.
    /// ¡Cambia estas credenciales si el proyecto sale de clase/pruebas!
    /// </summary>
    public static class DataSeeder
    {
        public const string AdminEmail = "admin@atm.com";
        public const string AdminPassword = "admin123";

        public static void Seed(IUserRepository users, IPasswordHasher hasher)
        {
            if (users.GetAll().Any(u => u.Role == UserRole.Admin)) return;

            users.Add(new User
            {
                FullName = "Administrador",
                Email = AdminEmail,
                PasswordHash = hasher.Hash(AdminPassword),
                Role = UserRole.Admin
            });
        }
    }
}
