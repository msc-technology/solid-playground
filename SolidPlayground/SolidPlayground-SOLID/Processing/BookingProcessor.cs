using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_SOLID.Repository;
using System.Text.Json;

namespace SolidPlayground_SOLID.Processing
{
    public class BookingProcessor : IMessageHandler
    {
        private readonly ILogger logger;
        private readonly IBookingEventRepository bookingEventRepository;

        public BookingProcessor(
            IBookingEventRepository iBookingEventRepository,
            ILogger iLogger
        )
        {
            logger = iLogger;
            bookingEventRepository = iBookingEventRepository;
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

            if (await bookingEventRepository.Exists(booking.BookingNumber))
            {
                logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                return;
            }

            if (!await bookingEventRepository.Store(booking))
            {
                logger.LogError("Error while storing Booking {@BookingNumber}", booking.BookingNumber);
            }
            else
            {
                logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
            }
        }
    }
}
