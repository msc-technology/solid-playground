using Core.DTO;
using SolidPlaygroundCore.Storage;
using SolidPlaygroundCore.Storage.Entities;

namespace SolidPlayground_S.Storage
{
    // Violates:
    // I: Interface segregation
    public class BookingEventRepository : IRepository<Booking?>
    {
        public async Task<bool> Exists(string key)
        {
            using (var db = new StorageContext())
            {
                var booking = await db.BookingEntity.FindAsync(key);
                return booking is not null;
            }
        }

        public async Task<Booking?> Find(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Store(Booking? message)
        {
            if (message == null || message.BookingNumber == null)
            {
                return false;
            }

            using (var db = new StorageContext())
            {
                var isBookingStored = await Exists(message.BookingNumber);
                if (!isBookingStored)
                {
                    await db.BookingEntity.AddAsync(new BookingEntity(message.BookingNumber));
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}
