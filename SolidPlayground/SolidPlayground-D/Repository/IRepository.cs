namespace SolidPlayground_D.Repository
{
    public interface IRepository<T>
    {
        Task Store(T message);
        Task<T> Find(string key);
        Task<bool> Exists(string key);
    }
}
