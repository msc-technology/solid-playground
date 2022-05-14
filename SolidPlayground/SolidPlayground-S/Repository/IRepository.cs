namespace SolidPlayground_S.Storage
{
    public interface IRepository<T>
    {
        Task<bool> Store(T message);
        Task<T> Find(string key);
        Task<bool> Exists(string key);
    }
}
