using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using Infrastructure.Logging;
using SolidPlayground_I.Repository;
using Infrastructure.Helper;

namespace SolidPlayground_I.Processing
{
    public class MessageProcessor : IMessageHandler
    {
        // Violates:
        // O: Open for change, closed to modification
        // D: Dipendency Inversion
        private readonly Publisher<EquipmentActivity> publisher;
        private readonly BookingEventRepository bookingEventRepository;
        private readonly EquipmentActivityEventRepository equipmentActivityEventRepository;
        private readonly IJsonHelper jsonHelper;
        private readonly ILogger logger;

        public MessageProcessor()
        {
            var publisherConnString = Environment.GetEnvironmentVariable("pub-connection-string") ?? "local-dev-string";
            publisher = new Publisher<EquipmentActivity>(new Subscription(publisherConnString));
            bookingEventRepository = new BookingEventRepository();
            equipmentActivityEventRepository = new EquipmentActivityEventRepository();
            jsonHelper = new JsonHelper();
            var loggerFactory = new LogServiceFactory();
            logger = loggerFactory.CreateLogger<MessageProcessor>();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Body))
            {
                logger.LogError("Invalid message received");
                return;
            }

            // equipment activities
            if (message.Body.Contains("ActivityId"))
            {
                EquipmentActivity? equipment = jsonHelper.Deserialize<EquipmentActivity>(message.Body);
                if (equipment is not null)
                {
                    if (!string.IsNullOrWhiteSpace(equipment.BookingNumber))
                    {
                        var isBookingFound = await equipmentActivityEventRepository.BookingExists(equipment.BookingNumber);
                        logger.LogInformation("Booking: {@BookingNumber} in equipment message {@Found}", equipment.BookingNumber,(equipment.BookingNumber is null ? "not found" : "exists"));
                        if (isBookingFound)
                        {
                            await publisher.Send(equipment);
                            logger.LogInformation("Equipment activity {@ActivityId} published", equipment.ActivityId);
                        }
                        else
                        {
                            await equipmentActivityEventRepository.Store(equipment);
                            logger.LogInformation("Stored equipment activity: {@message} with booking number not found", equipment);
                        }
                    }
                    else
                    {
                        logger.LogError("Equipment activity {@ActivityId} with missing booking number", equipment.ActivityId);
                    }
                }
            }
            // booking
            else if (message.Body.Contains("BookingNumber"))
            {
                Booking? booking = jsonHelper.Deserialize<Booking>(message.Body);
                if (booking is null)
                {
                    logger.LogError("Invalid booking received");
                }

                if (await bookingEventRepository.Exists(booking.BookingNumber))
                {
                    logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                    return;
                }

                await bookingEventRepository.Store(booking);
                logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
            }
            // error
            else
            {
                logger.LogError("Aborting processing for booking");
            }
        }
    }
}
