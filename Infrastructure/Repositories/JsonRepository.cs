using ATM2.Core.Interfaces;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    /// <summary>
    /// CRUD genérico sobre un archivo JSON. Los repositorios concretos heredan de aquí
    /// y solo agregan sus consultas específicas (GetByUserId, GetByEmail, etc.).
    /// </summary>
    public abstract class JsonRepository<T> where T : IEntity
    {
        protected readonly JsonService JsonService;
        protected readonly string FileName;

        protected JsonRepository(JsonService jsonService, string fileName)
        {
            JsonService = jsonService;
            FileName = fileName;
        }

        public List<T> GetAll() => JsonService.ReadAll<T>(FileName);

        public T? GetById(string id) => GetAll().FirstOrDefault(e => e.Id == id);

        public void Add(T entity)
        {
            var items = GetAll();
            if (items.Any(e => e.Id == entity.Id))
                throw new InvalidOperationException($"Ya existe un registro con id '{entity.Id}' en {FileName}.");

            items.Add(entity);
            Save(items);
        }

        public void Update(T entity)
        {
            var items = GetAll();
            int index = items.FindIndex(e => e.Id == entity.Id);
            if (index == -1)
                throw new InvalidOperationException($"No existe un registro con id '{entity.Id}' en {FileName}.");

            items[index] = entity;
            Save(items);
        }

        public void Delete(string id)
        {
            var items = GetAll();
            items.RemoveAll(e => e.Id == id);
            Save(items);
        }

        protected void Save(List<T> items) => JsonService.WriteAll(FileName, items);
    }
}
