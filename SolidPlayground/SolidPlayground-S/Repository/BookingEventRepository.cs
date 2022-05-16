using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_S.Repository
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

        public async Task Store(Booking? message)
        {
            using (var db = new StorageContext())
            {
                await db.BookingEntity.AddAsync(new BookingEntity(message.BookingNumber));
                db.SaveChanges();
            }
        }
    }
}
