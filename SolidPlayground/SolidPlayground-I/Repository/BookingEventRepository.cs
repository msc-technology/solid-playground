using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_I.Repository
{
    // Applies:
    // I: Interface segregation
    public class BookingEventRepository : BookingEventReadonlyRepository, IBookingEventRepository
    {
        public async Task<bool> StoreIfNotExists(Booking booking)
        {
            using (var db = new StorageContext())
            {
                if (await Exists(booking.BookingNumber))
                {
                    return false;
                }

                await db.AddAsync(new BookingEntity(booking.BookingNumber));
                await db.SaveChangesAsync();
                return true;
            }
        }
    }
}
