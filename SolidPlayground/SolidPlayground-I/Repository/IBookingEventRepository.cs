using Core.DTO;

namespace SolidPlayground_I.Repository
{
    public interface IBookingEventRepository
    {
        Task<bool> Exists(string key);
        Task Store(Booking booking);
    }
}
