using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using Infrastructure.Logging;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;
using Infrastructure.Helper;

namespace SolidPlayground_O.Processing
{
    public class EquipmentActivitiesProcessor : IMessageHandler
    {
        // Violates:
        // S: Single responsability
        // D: Dipendency Inversion
        private readonly Publisher<EquipmentActivity> publisher;
        private readonly JsonHelper jsonHelper;
        private readonly ILogger logger;

        public EquipmentActivitiesProcessor()
        {
            var publisherConnString = Environment.GetEnvironmentVariable("pub-connection-string") ?? "local-dev-string";
            publisher = new Publisher<EquipmentActivity>(new Subscription(publisherConnString));
            jsonHelper = new JsonHelper();
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

            EquipmentActivity? equipment = jsonHelper.Deserialize<EquipmentActivity>(message.Body);
            if (equipment is not null)
            {
                if (!string.IsNullOrWhiteSpace(equipment.BookingNumber))
                {
                    var isBookingFound = await BookingExists(equipment.BookingNumber);
                    logger.LogInformation("Booking: {@BookingNumber} in equipment message {@Found}", equipment.BookingNumber,(equipment.BookingNumber is null ? "not found" : "exists"));
                    if (isBookingFound)
                    {
                        await publisher.Send(equipment);
                        logger.LogInformation("Equipment activity {@ActivityId} published", equipment.ActivityId);
                    }
                    else
                    {
                        await StoreEquipment(equipment);
                        logger.LogInformation("Stored equipment activity: {@message} with booking number not found", equipment);
                    }
                }
                else
                {
                    logger.LogError("Equipment activity {@ActivityId} with missing booking number", equipment.ActivityId);
                }
            }
        }

        // db methods
        private async Task<bool> BookingExists(string bookingNumber)
        {
            using (var db = new StorageContext())
            {
                var booking = await db.BookingEntity.FindAsync(bookingNumber);
                return booking is not null;
            }
        }

        private async Task StoreEquipment(EquipmentActivity message)
        {
            using (var db = new StorageContext())
            {
                await db.EquipmentActivity.AddAsync(new EquipmentActivityEntity(message.ActivityId, message.BookingNumber));
                db.SaveChanges();
            }
        }
    }
}
