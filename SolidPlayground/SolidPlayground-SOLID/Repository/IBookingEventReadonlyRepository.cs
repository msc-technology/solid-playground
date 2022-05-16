namespace SolidPlayground_SOLID.Repository
{
    public interface IBookingEventReadonlyRepository
    {
        Task<bool> Exists(string key);
    }
}
