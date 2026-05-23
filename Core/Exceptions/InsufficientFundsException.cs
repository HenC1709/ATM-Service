namespace ATM2.Core.Exceptions
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(decimal amount)
            : base($"Insufficient funds to complete a transaction of {amount:C}") { }
    }
}