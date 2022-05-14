using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_SOLID.Repository
{
    public class BookingEventRepository : BookingEventReadonlyRepository, IBookingEventRepository
    {
        public async Task<bool> Store(Booking booking)
        {
            if (booking is null)
            {
                return false;
            }

            using (var db = new StorageContext())
            {
                await db.AddAsync(new BookingEntity(booking.BookingNumber));
                await db.SaveChangesAsync();
                return true;
            }
        }
    }
}
