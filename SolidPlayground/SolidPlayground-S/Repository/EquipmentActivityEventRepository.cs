using Core.DTO;
using SolidPlaygroundCore.Storage;
using SolidPlaygroundCore.Storage.Entities;

namespace SolidPlayground_S.Storage
{
    // Violates:
    // I: Interface segregation
    public class EquipmentActivityEventRepository : IRepository<EquipmentActivity?>
    {
        public async Task<bool> Exists(string key)
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

        public async Task<bool> Store(EquipmentActivity? message)
        {
            if (message is null)
            {
                return false;
            }

            using (var db = new StorageContext())
            {
                await db.EquipmentActivity.AddAsync(new EquipmentActivityEntity(message.ActivityId, message.BookingNumber));
                db.SaveChanges();
                return true;
            }
        }
    }
}
