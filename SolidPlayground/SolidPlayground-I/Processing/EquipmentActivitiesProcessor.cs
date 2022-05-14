using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_I.Repository;
using Infrastructure.Logging;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;
using System.Text.Json;

namespace SolidPlayground_I.Processing
{
    public class EquipmentActivitiesProcessor : IMessageHandler
    {
        // Violates:
        // D: Dipendency Inversion
        private readonly Publisher<EquipmentActivity> publisher;
        private readonly ILogger logger;
        private readonly BookingEventReadonlyRepository BookingEventRepository;

        public EquipmentActivitiesProcessor()
        {
            var publisherConnString = Environment.GetEnvironmentVariable("pub-connection-string") ?? "local-dev-string";
            publisher = new Publisher<EquipmentActivity>(new Subscription(publisherConnString));
            var loggerFactory = new LogServiceFactory();
            logger = loggerFactory.CreateLogger<EquipmentActivitiesProcessor>();
            BookingEventRepository = new BookingEventReadonlyRepository();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null)
            {
                logger.LogError("Invalid message received");
                return;
            }

            string body = message.Body;
            if (string.IsNullOrEmpty(body))
            {
                logger.LogError("Invalid message received");
                return;
            }

            // equipment activities
            if (body.Contains("ActivityId"))
            {
                EquipmentActivity equipment = JsonSerializer.Deserialize<EquipmentActivity>(body);
                if (equipment is not null)
                {
                    if (!string.IsNullOrWhiteSpace(equipment.BookingNumber))
                    {
                        var isBookingFound = await BookingEventRepository.Exists(equipment.BookingNumber);
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
            // booking
            else if (body.Contains("BookingNumber"))
            {
               
            }
            // error
            else
            {
                logger.LogError("Aborting processing for booking");
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
