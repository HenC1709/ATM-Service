namespace ATM2.Core.Exceptions
{
    public class InvalidPinException : AtmException
    {
        public int RemainingAttempts { get; }

        public InvalidPinException(int remainingAttempts)
            : base($"PIN incorrecto. Intentos restantes: {remainingAttempts}.")
        {
            RemainingAttempts = remainingAttempts;
        }
    }
}
