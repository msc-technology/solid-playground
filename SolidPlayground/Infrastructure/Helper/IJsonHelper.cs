namespace Infrastructure.Helper
{
    public interface IJsonHelper
    {
        T? Deserialize<T>(string json);
    }
}
