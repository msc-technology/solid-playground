namespace SolidPlayground_S.Repository
{
    public interface IRepository<T>
    {
        Task Store(T message);
        Task<T> Find(string key);
        Task<bool> Exists(string key);
    }
}
