namespace ATM2.Core.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string userId)
            : base($"User with id '{userId}' was not found") { }
    }
}