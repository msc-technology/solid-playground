using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_L.Storage;
using SolidPlaygroundCore.Infrastructure;
using System.Text.Json;

namespace SolidPlayground_L.Processing
{
    public class BookingProcessor : IMessageHandler
    {
        // Violates:
        // D: Dipendency Inversion
        private readonly ILogger Logger;
        private readonly BookingEventRepository BookingEventRepository;

        public BookingProcessor()
        {
            var loggerFactory = new LoggerFactory();
            Logger = loggerFactory.CreateLogger<BookingProcessor>();
            Logger = new LogServiceFactory().CreateLogger<BookingProcessor>();
            BookingEventRepository = new BookingEventRepository();
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
                if (!await BookingEventRepository.Store(booking))
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
    }
}
