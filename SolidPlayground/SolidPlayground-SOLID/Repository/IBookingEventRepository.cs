using Core.DTO;

namespace SolidPlayground_SOLID.Repository
{
    public interface IBookingEventRepository : IBookingEventReadonlyRepository
    {
        Task Store(Booking booking);
    }
}
