using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_I.Repository
{
    // Applies:
    // I: Interface segregation
    public class BookingEventRepository : BookingEventReadonlyRepository, IBookingEventRepository
    {
        public async Task Store(Booking booking)
        {
            using (var db = new StorageContext())
            {
                await db.AddAsync(new BookingEntity(booking.BookingNumber));
                await db.SaveChangesAsync();
            }
        }
    }
}
