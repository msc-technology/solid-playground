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

        public async Task Store(EquipmentActivity? message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            using (var db = new StorageContext())
            {
                await db.EquipmentActivity.AddAsync(new EquipmentActivityEntity(message.ActivityId, message.BookingNumber));
                db.SaveChanges();
            }
        }
    }
}
