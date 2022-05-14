using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_I.Repository
{
    // Applies:
    // I: Interface segregation
    public class EquipmentActivityEventRepository : IEquipmentActivityRepository
    {
        private readonly BookingEventReadonlyRepository bookingEventReadonlyRepository;

        public EquipmentActivityEventRepository()
        {
            bookingEventReadonlyRepository = new BookingEventReadonlyRepository();
        }

        public async Task<bool> BookingExists(string key)
        {
            return await bookingEventReadonlyRepository.Exists(key);
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
