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

        private readonly string _basePath;

        public JsonService(string basePath)
        {
            _basePath = basePath;
            Directory.CreateDirectory(_basePath); // evita DirectoryNotFoundException al escribir
        }

        public List<T> ReadAll<T>(string fileName)
        {
            string path = GetPath(fileName);
            if (!File.Exists(path)) return new List<T>();

            string json = File.ReadAllText(path);
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

            // Escritura "atómica": si el programa se cierra a mitad, el archivo original queda intacto.
            File.WriteAllText(tempPath, JsonSerializer.Serialize(data, Options));
            File.Move(tempPath, path, overwrite: true);
        }

        private string GetPath(string fileName) => Path.Combine(_basePath, fileName);
    }
}
