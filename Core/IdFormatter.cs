namespace ATM2.Core
{
    public static class IdFormatter
    {
        /// <summary>Versión corta (8 caracteres en mayúsculas) de un GUID para mostrar al usuario.</summary>
        public static string Short(string id) =>
            id.Length <= 8 ? id.ToUpperInvariant() : id[..8].ToUpperInvariant();
    }
}
