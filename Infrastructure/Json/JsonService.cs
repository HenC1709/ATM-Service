using System.Text.Json;

namespace ATM2.Infrastructure.Json
{
    public class JsonService
    {
        private readonly string _basePath;
        public JsonService(string basePath)
        {
            _basePath = basePath;
        }
       public List<T> ReadALL<T>(string fileName)
        {
            string path = Path.Combine(_basePath, fileName);
            if (!File.Exists(path)) return new List<T>();
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
       public void WriteAll<T>(string fileName, List<T> data)
        {
            string path = Path.Combine(_basePath, fileName);
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

    }
}