using System.Text.Json;

namespace Infrastructure.Helper
{
    public class JsonHelper : IJsonHelper
    {
        public T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
