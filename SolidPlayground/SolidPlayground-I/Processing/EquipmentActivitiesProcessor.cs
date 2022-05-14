using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_I.Repository;
using Infrastructure.Logging;
using System.Text.Json;

namespace SolidPlayground_I.Processing
{
    public class EquipmentActivitiesProcessor : IMessageHandler
    {
        // Violates:
        // D: Dipendency Inversion
        private readonly Publisher<EquipmentActivity> publisher;
        private readonly BookingEventReadonlyRepository bookingEventRepository;
        private readonly EquipmentActivityEventRepository equipmentActivityRepository;
        private readonly ILogger logger;

        public EquipmentActivitiesProcessor()
        {
            var publisherConnString = Environment.GetEnvironmentVariable("pub-connection-string") ?? "local-dev-string";
            publisher = new Publisher<EquipmentActivity>(new Subscription(publisherConnString));
            bookingEventRepository = new BookingEventReadonlyRepository();
            equipmentActivityRepository = new EquipmentActivityEventRepository();
            var loggerFactory = new LogServiceFactory();
            logger = loggerFactory.CreateLogger<EquipmentActivitiesProcessor>();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Body))
            {
                logger.LogError("Invalid message received");
                return;
            }

            EquipmentActivity? equipment = JsonSerializer.Deserialize<EquipmentActivity>(message.Body);
            if (equipment is not null)
            {
                if (!string.IsNullOrWhiteSpace(equipment.BookingNumber))
                {
                    var isBookingFound = await bookingEventRepository.Exists(equipment.BookingNumber);
                    logger.LogInformation("Booking: {@BookingNumber} in equipment message {@Found}", equipment.BookingNumber,(equipment.BookingNumber is null ? "not found" : "exists"));
                    if (isBookingFound)
                    {
                        await publisher.Send(equipment);
                        logger.LogInformation("Equipment activity {@ActivityId} published", equipment.ActivityId);
                    }
                    else
                    {
                        await equipmentActivityRepository.Store(equipment);
                        logger.LogInformation("Stored equipment activity: {@message} with booking number not found", equipment);
                    }
                }
                else
                {
                    logger.LogError("Equipment activity {@ActivityId} with missing booking number", equipment.ActivityId);
                }
            }
        }
    }
}
