using Infrastructure.Storage;

namespace SolidPlayground_SOLID.Repository
{
    public class BookingEventReadonlyRepository : IBookingEventReadonlyRepository
    {
        public async Task<bool> Exists(string key)
        {
            using (var db = new StorageContext())
            {
                var booking = await db.BookingEntity.FindAsync(key);
                return booking is not null;
            }
        }
    }
}
