namespace ATM2.Core.Exceptions
{
    public class InvalidSessionException : AtmException
    {
        public InvalidSessionException()
            : base("La sesión no es válida o ha expirado. Inicia sesión de nuevo.") { }
    }
}
