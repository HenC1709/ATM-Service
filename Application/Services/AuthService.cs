using System.Net.Mail;
using ATM2.Core;
using ATM2.Core.Enums;
using ATM2.Core.Exceptions;
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Security;
using ATM2.Core.Interfaces.Services;
using ATM2.Core.Models;

namespace ATM2.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IAccountService _accounts;
        private readonly ISessionService _sessions;
        private readonly IPasswordHasher _hasher;

        public AuthService(IUserRepository users, IAccountService accounts, ISessionService sessions, IPasswordHasher hasher)
        {
            _users = users;
            _accounts = accounts;
            _sessions = sessions;
            _hasher = hasher;
        }

        public User Register(string fullName, string email, string password)
        {
            fullName = fullName.Trim();
            email = NormalizeEmail(email);

            if (fullName.Length < 3)
                throw new AtmException("El nombre debe tener al menos 3 caracteres.");
            if (!MailAddress.TryCreate(email, out _) || !email.Contains('.'))
                throw new AtmException("El correo no tiene un formato válido.");
            if (password.Length < AtmSettings.MinPasswordLength)
                throw new AtmException($"La contraseña debe tener al menos {AtmSettings.MinPasswordLength} caracteres.");
            if (_users.GetByEmail(email) is not null)
                throw new AtmException("Ya existe un usuario registrado con ese correo.");

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = _hasher.Hash(password),
                Role = UserRole.Client
            };
            _users.Add(user);
            _accounts.Create(user.Id);
            return user;
        }

        public Session Login(string email, string password)
        {
            var user = _users.GetByEmail(NormalizeEmail(email))
                ?? throw new InvalidCredentialsException();

            if (user.IsLocked)
                throw new AccountLockedException();

            if (!_hasher.Verify(password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                // Los administradores no se bloquean para no dejar el sistema sin nadie que desbloquee.
                if (user.Role == UserRole.Client && user.FailedLoginAttempts >= AtmSettings.MaxLoginAttempts)
                {
                    user.IsLocked = true;
                    _users.Update(user);
                    throw new AccountLockedException();
                }

                _users.Update(user);
                if (user.Role == UserRole.Admin) throw new InvalidCredentialsException();
                throw new InvalidCredentialsException(AtmSettings.MaxLoginAttempts - user.FailedLoginAttempts);
            }

            user.FailedLoginAttempts = 0;
            user.LastLoginAt = DateTime.UtcNow;
            _users.Update(user);

            // Una sola sesión activa por usuario.
            var existing = _sessions.GetActive(user.Id);
            if (existing is not null) _sessions.End(existing.Id);

            return _sessions.Create(user.Id);
        }

        public void Logout(string sessionId) => _sessions.End(sessionId);

        public bool IsSessionActive(string sessionId) => _sessions.IsActive(sessionId);

        private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
    }
}
