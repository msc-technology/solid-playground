using Core.DTO;

namespace SolidPlayground_SOLID.Repository
{
    public interface IBookingEventRepository
    {
        Task<bool> Exists(string key);
        Task Store(Booking booking);
    }
}
