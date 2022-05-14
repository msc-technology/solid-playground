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
            try
            {
                Booking? booking = JsonSerializer.Deserialize<Booking>(message.Body);
                if (await bookingEventRepository.Exists(booking.BookingNumber))
                {
                    logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                    return;
                }

                await bookingEventRepository.Store(booking);
                logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
            } 
            catch (ArgumentNullException ex)
            {
                logger.LogError("Invalid argument {@ex}", ex);
            } 
            catch (JsonException ex)
            {
                logger.LogError("Invalid json {@ex}", ex);
            }
        }
    }
}
