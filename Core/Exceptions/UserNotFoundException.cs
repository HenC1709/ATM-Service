namespace ATM2.Core.Exceptions
{
    public class UserNotFoundException : AtmException
    {
        public UserNotFoundException(string identifier)
            : base($"No se encontró el usuario '{identifier}'.") { }
    }
}
