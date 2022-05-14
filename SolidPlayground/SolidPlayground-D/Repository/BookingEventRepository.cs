using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_D.Repository
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

        public Task<Booking?> Find(string key)
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
                await db.BookingEntity.AddAsync(new BookingEntity(message.BookingNumber));
                db.SaveChanges();
                return true;
            }
        }
    }
}
