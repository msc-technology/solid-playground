using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_D.Repository
{
    // Violates:
    // I: Interface segregation
    public class EquipmentActivityEventRepository : IRepository<EquipmentActivity?>
    {
        public Task<bool> Exists(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<EquipmentActivity?> Find(string key)
        {
            if (key is null)
            {
                return null;
            }

            using (var db = new StorageContext())
            {
                var activity = await db.EquipmentActivity.FindAsync(typeof(EquipmentActivity), key);
                return activity is not null ? new EquipmentActivity(activity.ActivityId, activity.BookingNumber) : null;
            }
        }

        public async Task Store(EquipmentActivity? message)
        {
            using (var db = new StorageContext())
            {
                await db.EquipmentActivity.AddAsync(new EquipmentActivityEntity(message.ActivityId, message.BookingNumber));
                db.SaveChanges();
            }
        }
    }
}
