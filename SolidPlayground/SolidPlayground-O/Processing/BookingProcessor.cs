using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;
using System.Text.Json;
using Infrastructure.Logging;

namespace SolidPlayground_O.Processing
{
    public class BookingProcessor : IMessageHandler
    {
        // Violates:
        // D: Dipendency Inversion
        private readonly ILogger logger;

        public BookingProcessor()
        {
            var loggerFactory = new LogServiceFactory();
            logger = loggerFactory.CreateLogger<BookingProcessor>();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Body))
            {
                logger.LogError("Invalid message received");
                return;
            }

            string body = message.Body;

            Booking booking = JsonSerializer.Deserialize<Booking>(body);
            if (booking is null)
            {
                logger.LogError("Invalid booking received");
            }

            if (await BookingExists(booking.BookingNumber))
            {
                logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                return;
            }

            if (!await StoreBooking(booking))
            {
                logger.LogError("Error while storing Booking {@BookingNumber}", booking.BookingNumber);
            }
            else
            {
                logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
            }
        }

        private async Task<bool> StoreBooking(Booking message)
        {
            using (var db = new StorageContext())
            {
                var isBookingStored = await BookingExists(message.BookingNumber);
                if (isBookingStored)
                {
                    return false;
                }
                else
                {
                    await db.BookingEntity.AddAsync(new BookingEntity(message.BookingNumber));
                    db.SaveChanges();
                    return true;
                }
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
