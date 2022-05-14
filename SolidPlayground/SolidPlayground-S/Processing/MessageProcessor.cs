using MessagesFramework;
using Core.DTO;
using SolidPlayground_S.Repository;
using System.Text.Json;
using Infrastructure.Logging;
using Microsoft.Extensions.Logging;

namespace SolidPlayground_S.Processing
{
    public class MessageProcessor : IMessageHandler
    {
        // Violates:
        // O: Open for change, closed to modification
        // D: Dipendency Inversion
        private readonly Publisher<EquipmentActivity> publisher;
        private readonly ILogger logger;
        private readonly BookingEventRepository bookingEventRepository;
        private readonly EquipmentActivityEventRepository equipmentActivityEventStorage;

        public MessageProcessor()
        {
            var publisherConnString = Environment.GetEnvironmentVariable("pub-connection-string") ?? "local-dev-string";
            publisher = new Publisher<EquipmentActivity>(new Subscription(publisherConnString));
            logger = new LogServiceFactory().CreateLogger<MessageProcessor>();
            bookingEventRepository = new BookingEventRepository();
            equipmentActivityEventStorage = new EquipmentActivityEventRepository();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Body))
            {
                logger.LogError("Invalid message received");
                return;
            }

            string body = message.Body;

            // equipment activities
            if (body.Contains("ActivityId"))
            {
                EquipmentActivity equipment = JsonSerializer.Deserialize<EquipmentActivity>(body);
                if (equipment is not null)
                {
                    if (!string.IsNullOrWhiteSpace(equipment.BookingNumber))
                    {
                        var isBookingFound = await bookingEventRepository.Exists(equipment.BookingNumber);
                        if (isBookingFound)
                        {
                            await publisher.Send(equipment);
                            logger.LogInformation("Equipment activity {@ActivityId} published", equipment.ActivityId);
                        }
                        else
                        {
                            await equipmentActivityEventStorage.Store(equipment);
                            logger.LogError("Equipment activity {@ActivityId} with booking number {@BookingNumber} not found", equipment.ActivityId, equipment.BookingNumber);
                        }
                    }
                    else
                    {
                        logger.LogError("Equipment activity {@ActivityId} with missing booking number", equipment.ActivityId);
                    }
                }
            }
            // booking
            else if (body.Contains("BookingNumber"))
            {
                Booking booking = JsonSerializer.Deserialize<Booking>(body);
                if (booking is not null)
                {
                    if (!await bookingEventRepository.Store(booking))
                    {
                        logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                    }
                    else
                    {
                        logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
                    }
                }
                else
                {
                    logger.LogError("Invalid booking received");
                }
            }
            // error
            else
            {
                logger.LogError("Aborting processing for booking");
            }
        }
    }
}
