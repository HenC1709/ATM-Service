using System.Text.Json;
using System.Text.Json.Serialization;

namespace ATM2.Infrastructure.Json
{
    public class JsonService
    {
        // Se crea una sola vez: crear JsonSerializerOptions en cada llamada es costoso.
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() } // enums como texto ("Withdrawal") en vez de números
        };

        // Caché del texto JSON de cada archivo. Se guarda el texto (no los objetos) para que cada lectura
        // devuelva instancias nuevas y modificar una entidad no altere los datos hasta llamar a Update.
        private sealed record CachedFile(string Json, DateTime LastWriteUtc);
        private readonly Dictionary<string, CachedFile> _cache = new();
        private readonly object _lock = new();

        private readonly string _basePath;

        public JsonService(string basePath)
        {
            _basePath = basePath;
            Directory.CreateDirectory(_basePath); // evita DirectoryNotFoundException al escribir
        }

        public List<T> ReadAll<T>(string fileName)
        {
            string json = ReadText(fileName);
            if (string.IsNullOrWhiteSpace(json)) return new List<T>();

            try
            {
                return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
            }
            catch (JsonException ex)
            {
                // No devolvemos una lista vacía: el siguiente WriteAll borraría todos los datos.
                throw new InvalidOperationException($"El archivo de datos '{fileName}' está dañado: {ex.Message}", ex);
            }
        }

        public void WriteAll<T>(string fileName, List<T> data)
        {
            string path = GetPath(fileName);
            string tempPath = path + ".tmp";
            string json = JsonSerializer.Serialize(data, Options);

            lock (_lock)
            {
                // Escritura "atómica": si el programa se cierra a mitad, el archivo original queda intacto.
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, path, overwrite: true);
                _cache[fileName] = new CachedFile(json, File.GetLastWriteTimeUtc(path));
            }
        }

        /// <summary>Lee el archivo solo si cambió en disco desde la última lectura/escritura.</summary>
        private string ReadText(string fileName)
        {
            string path = GetPath(fileName);

            lock (_lock)
            {
                if (!File.Exists(path))
                {
                    _cache.Remove(fileName);
                    return string.Empty;
                }

                DateTime lastWrite = File.GetLastWriteTimeUtc(path);
                if (_cache.TryGetValue(fileName, out var cached) && cached.LastWriteUtc == lastWrite)
                    return cached.Json;

                string json = File.ReadAllText(path);
                _cache[fileName] = new CachedFile(json, lastWrite);
                return json;
            }
        }

        private string GetPath(string fileName) => Path.Combine(_basePath, fileName);
    }
}
