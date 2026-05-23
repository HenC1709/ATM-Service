namespace ATM2.Core.Exceptions
{
    public class InvalidSessionException : Exception
    {
        public InvalidSessionException()
            : base("Session is invalid or has expired") { }
    }
}