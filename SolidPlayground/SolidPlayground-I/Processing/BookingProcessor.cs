using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_I.Repository;
using Infrastructure.Logging;
using System.Text.Json;

namespace SolidPlayground_I.Processing
{
    public class BookingProcessor : IMessageHandler
    {
        // Violates:
        // D: Dipendency Inversion
        private readonly ILogger logger;
        private readonly BookingEventRepository BookingEventRepository;

        public BookingProcessor()
        {
            var loggerFactory = new LogServiceFactory();
            logger = loggerFactory.CreateLogger<BookingProcessor>();
            BookingEventRepository = new BookingEventRepository();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Body))
            {
                logger.LogError("Invalid message received");
                return;
            }

            Booking? booking = JsonSerializer.Deserialize<Booking>(message.Body);
            if (booking is null)
            {
                logger.LogError("Invalid booking received");
                return;
            }

            if (await BookingEventRepository.Exists(booking.BookingNumber))
            {
                logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                return;
            }

            await BookingEventRepository.Store(booking);
            logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
        }
    }
}
