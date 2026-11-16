using ATM2.Core.Enums;
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Security;
using ATM2.Core.Models;

namespace ATM2.Infrastructure.Seed
{
    /// <summary>
    /// Crea un administrador la primera vez que se ejecuta la app.
    /// Las credenciales se pueden definir con las variables de entorno
    /// ATM_ADMIN_EMAIL y ATM_ADMIN_PASSWORD; si no existen se usan las de desarrollo.
    /// </summary>
    public static class DataSeeder
    {
        public const string DefaultAdminEmail = "admin@atm.com";
        public const string DefaultAdminPassword = "admin123";

        public static void Seed(IUserRepository users, IPasswordHasher hasher)
        {
            if (users.GetAll().Any(u => u.Role == UserRole.Admin)) return;

            string email = Environment.GetEnvironmentVariable("ATM_ADMIN_EMAIL") ?? DefaultAdminEmail;
            string password = Environment.GetEnvironmentVariable("ATM_ADMIN_PASSWORD") ?? DefaultAdminPassword;

            users.Add(new User
            {
                FullName = "Administrador",
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = hasher.Hash(password),
                Role = UserRole.Admin
            });
        }
    }
}
