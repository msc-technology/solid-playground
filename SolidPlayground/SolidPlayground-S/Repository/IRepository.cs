namespace SolidPlayground_S.Repository
{
    public interface IRepository<T>
    {
        Task<bool> Store(T message);
        Task<T> Find(string key);
        Task<bool> Exists(string key);
    }
}
