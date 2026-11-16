using ATM2.Core.Exceptions;

namespace ATM2.Core
{
    /// <summary>Validaciones reutilizables para no repetir las mismas comprobaciones en cada servicio.</summary>
    public static class Guard
    {
        public static void Positive(decimal amount)
        {
            if (amount <= 0)
                throw new AtmException("El monto debe ser mayor que cero.");
        }
    }
}
