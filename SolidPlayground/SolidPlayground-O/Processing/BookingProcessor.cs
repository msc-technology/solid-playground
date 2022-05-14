using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlaygroundCore.Infrastructure;
using SolidPlaygroundCore.Storage;
using SolidPlaygroundCore.Storage.Entities;
using System.Text.Json;

namespace SolidPlayground_O.Processing
{
    public class BookingProcessor : IMessageHandler
    {
        // Violates:
        // D: Dipendency Inversion
        private readonly ILogger Logger;

        public BookingProcessor()
        {
            var loggerFactory = new LoggerFactory();
            Logger = loggerFactory.CreateLogger<BookingProcessor>();
            Logger = new LogServiceFactory().CreateLogger<BookingProcessor>();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Body))
            {
                Logger.LogError("Invalid message received");
                return;
            }

            string body = message.Body;
           
            Booking booking = JsonSerializer.Deserialize<Booking>(body);
            if (booking is not null)
            {
                if (!await StoreBooking(booking))
                {
                    Logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                }
                else
                {
                    Logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
                }
            }
            else
            {
                Logger.LogError("Invalid booking received");
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
