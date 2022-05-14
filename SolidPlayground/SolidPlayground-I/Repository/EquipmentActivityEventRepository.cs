using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_I.Repository
{
    // Applies:
    // I: Interface segregation
    public class EquipmentActivityEventRepository : IEquipmentActivityRepository
    {
        private readonly BookingEventReadonlyRepository BookingEventReadonlyRepository;

        public EquipmentActivityEventRepository()
        {
            BookingEventReadonlyRepository = new BookingEventReadonlyRepository();
        }
        public async Task<bool> BookingExists(string key)
        {
            Console.WriteLine("check booking");
            return await BookingEventReadonlyRepository.Exists(key);
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
