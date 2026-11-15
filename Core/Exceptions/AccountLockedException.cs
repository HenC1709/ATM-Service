namespace ATM2.Core.Exceptions
{
    public class AccountLockedException : AtmException
    {
        public AccountLockedException()
            : base("Tu usuario está bloqueado por demasiados intentos fallidos. Contacta a un administrador.") { }
    }
}
