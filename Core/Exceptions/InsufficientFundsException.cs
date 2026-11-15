namespace ATM2.Core.Exceptions
{
    public class InsufficientFundsException : AtmException
    {
        public InsufficientFundsException(decimal amount)
            : base($"Fondos insuficientes para completar una operación de {amount:C}.") { }
    }
}
