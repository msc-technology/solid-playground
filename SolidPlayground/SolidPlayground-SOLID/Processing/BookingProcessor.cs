using Core.DTO;
using Infrastructure.Helper;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_SOLID.Repository;

namespace SolidPlayground_SOLID.Processing
{
    public class BookingProcessor : IMessageHandler
    {
        private readonly IBookingEventRepository bookingEventRepository;
        private readonly IJsonHelper jsonHelper;
        private readonly ILogger logger;

        public BookingProcessor(
            IBookingEventRepository iBookingEventRepository,
            IJsonHelper ijsonHelper,
            ILogger iLogger
        )
        {
            bookingEventRepository = iBookingEventRepository;
            jsonHelper = ijsonHelper;
            logger = iLogger;
        }

        public async Task HandleMessage(Message message)
        {

            Booking? booking = jsonHelper.Deserialize<Booking>(message.Body);
            if (await bookingEventRepository.Exists(booking.BookingNumber))
            {
                logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                return;
            }

            await bookingEventRepository.Store(booking);
            logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
        }
    }
}
