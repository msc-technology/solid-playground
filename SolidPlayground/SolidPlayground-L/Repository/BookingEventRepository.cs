using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_L.Repository
{
    // Applies:
    // L: Liskov Substitution Principle
    // it uses correctly inheritance: if it was done the other way around (BookingEventReadonlyRepository : BookingEventRepository)
    // then some methods would have to be reimplemented to do nothing or throw an exception to allow the class to respect "readonly" requirement
    public class BookingEventRepository : BookingEventReadonlyRepository
    {
        public async Task Store(Booking? message)
        {
            using (var db = new StorageContext())
            {
                await db.BookingEntity.AddAsync(new BookingEntity(message.BookingNumber));
                db.SaveChanges();
            }
        }
    }
}
