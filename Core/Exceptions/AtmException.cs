namespace ATM2.Core.Exceptions
{
    /// <summary>
    /// Clase base para los errores de negocio del cajero.
    /// La interfaz de consola captura AtmException y muestra el mensaje al usuario.
    /// </summary>
    public class AtmException : Exception
    {
        public AtmException(string message) : base(message) { }
    }
}
