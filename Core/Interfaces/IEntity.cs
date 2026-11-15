namespace ATM2.Core.Interfaces
{
    /// <summary>
    /// Toda entidad que se guarda en JSON tiene un Id único.
    /// Permite reutilizar el repositorio genérico JsonRepository&lt;T&gt;.
    /// </summary>
    public interface IEntity
    {
        string Id { get; set; }
    }
}
