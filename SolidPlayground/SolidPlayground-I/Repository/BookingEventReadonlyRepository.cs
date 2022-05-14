using Infrastructure.Storage;

namespace SolidPlayground_I.Repository
{
    public class BookingEventReadonlyRepository
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
