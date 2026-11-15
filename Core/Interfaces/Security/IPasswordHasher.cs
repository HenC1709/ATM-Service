namespace ATM2.Core.Interfaces.Security
{
    /// <summary>
    /// Hashea y verifica secretos (contraseñas y PINs) para no guardarlos en texto plano.
    /// </summary>
    public interface IPasswordHasher
    {
        string Hash(string value);
        bool Verify(string value, string hash);
    }
}
