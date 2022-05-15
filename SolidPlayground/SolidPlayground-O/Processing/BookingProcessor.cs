using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;
using Infrastructure.Logging;
using Infrastructure.Helper;

namespace SolidPlayground_O.Processing
{
    public class BookingProcessor : IMessageHandler
    {
        // Violates:
        // S: Single responsability
        // D: Dipendency Inversion
        private readonly JsonHelper jsonHelper;
        private readonly ILogger logger;

        public BookingProcessor()
        {
            jsonHelper = new JsonHelper();
            var loggerFactory = new LogServiceFactory();
            logger = loggerFactory.CreateLogger<BookingProcessor>();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Body))
            {
                logger.LogError("Invalid message received");
                return;
            }

            Booking? booking = jsonHelper.Deserialize<Booking>(message.Body);
            if (booking is null)
            {
                logger.LogError("Invalid booking received");
                return;
            }

            if (await BookingExists(booking.BookingNumber))
            {
                logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                return;
            }

            await StoreBooking(booking);
            logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
        }

        private async Task StoreBooking(Booking message)
        {
            using (var db = new StorageContext())
            {
                await db.BookingEntity.AddAsync(new BookingEntity(message.BookingNumber));
                db.SaveChanges();
            }
        }

        private async Task<bool> BookingExists(string bookingNumber)
        {
            using (var db = new StorageContext())
            {
                var booking = await db.BookingEntity.FindAsync(bookingNumber);
                return booking is not null;
            }
        }
    }
}
