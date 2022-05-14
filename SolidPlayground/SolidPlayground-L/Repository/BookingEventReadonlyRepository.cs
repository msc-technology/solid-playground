using SolidPlaygroundCore.Storage;

namespace SolidPlayground_L.Storage
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
