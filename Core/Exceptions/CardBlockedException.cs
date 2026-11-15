namespace ATM2.Core.Exceptions
{
    public class CardBlockedException : AtmException
    {
        public CardBlockedException()
            : base("La tarjeta está bloqueada. Contacta a un administrador para desbloquearla.") { }
    }
}
