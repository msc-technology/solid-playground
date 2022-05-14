using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_SOLID.Repository
{
    public class EquipmentActivityEventRepository : IEquipmentActivityEventRepository
    {
        private readonly IBookingEventReadonlyRepository bookingEventReadonlyRepository;

        public EquipmentActivityEventRepository(IBookingEventReadonlyRepository bookingReadonlyRepository)
        {
            bookingEventReadonlyRepository = bookingReadonlyRepository;
        }

        public async Task<bool> BookingExists(string key)
        {
            return await bookingEventReadonlyRepository.Exists(key);
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
