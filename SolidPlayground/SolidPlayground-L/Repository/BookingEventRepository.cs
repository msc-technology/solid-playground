using Core.DTO;
using SolidPlaygroundCore.Storage;
using SolidPlaygroundCore.Storage.Entities;

namespace SolidPlayground_L.Storage
{
    // Applies:
    // L: Liskov Substitution Principle
    public class BookingEventRepository : BookingEventReadonlyRepository
    {
        public async Task<bool> Store(Booking? message)
        {
            if (message == null || message.BookingNumber == null)
            {
                return false;
            }

            using (var db = new StorageContext())
            {
                var isBookingStored = await Exists(message.BookingNumber);
                if (!isBookingStored)
                {
                    await db.BookingEntity.AddAsync(new BookingEntity(message.BookingNumber));
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}
