namespace ATM2.Core.Exceptions
{
    public class InvalidCredentialsException : AtmException
    {
        public InvalidCredentialsException()
            : base("Correo o contraseña incorrectos.") { }

        public InvalidCredentialsException(int remainingAttempts)
            : base($"Correo o contraseña incorrectos. Intentos restantes: {remainingAttempts}.") { }
    }
}
