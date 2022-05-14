using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_L.Repository;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;
using System.Text.Json;
using Infrastructure.Logging;

namespace SolidPlayground_L.Processing
{
    public class EquipmentActivitiesProcessor : IMessageHandler
    {
        // Violates:
        // D: Dipendency Inversion
        private readonly Publisher<EquipmentActivity> publisher;
        private readonly ILogger logger;
        private readonly BookingEventReadonlyRepository bookingEventRepository;

        public EquipmentActivitiesProcessor()
        {
            var publisherConnString = Environment.GetEnvironmentVariable("pub-connection-string") ?? "local-dev-string";
            var loggerFactory = new LogServiceFactory();
            logger = loggerFactory.CreateLogger<EquipmentActivitiesProcessor>();
            publisher = new Publisher<EquipmentActivity>(new Subscription(publisherConnString));
            bookingEventRepository = new BookingEventReadonlyRepository();
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
                        if (await StoreEquipment(equipment))
                        {
                            logger.LogInformation("Stored equipment activity: {@message} with booking number not found", equipment);
                        }
                        else
                        {
                            logger.LogError("Error while storing equipment activity");
                        }
                    }
                }
                else
                {
                    logger.LogError("Equipment activity {@ActivityId} with missing booking number", equipment.ActivityId);
                }
            }
        }

        // db methods
        private async Task<bool> StoreEquipment(EquipmentActivity message)
        {
            if (message is null)
            {
                return false;
            }

            using (var db = new StorageContext())
            {
                await db.EquipmentActivity.AddAsync(new EquipmentActivityEntity(message.ActivityId, message.BookingNumber));
                db.SaveChanges();
                return true;
            }
        }
    }
}
