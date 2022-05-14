using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlaygroundCore.Infrastructure;
using SolidPlaygroundCore.Storage;
using SolidPlaygroundCore.Storage.Entities;
using System.Text.Json;

namespace SolidPlayground_O.Processing
{
    public class EquipmentActivitiesProcessor : IMessageHandler
    {
        // Violates:
        // D: Dipendency Inversion
        private readonly Publisher<EquipmentActivity> Publisher;
        private readonly ILogger Logger;

        public EquipmentActivitiesProcessor()
        {
            var loggerFactory = new LoggerFactory();
            Logger = loggerFactory.CreateLogger<EquipmentActivitiesProcessor>();
            var publisherConnString = Environment.GetEnvironmentVariable("pub-connection-string") ?? "local-dev-string";
            Publisher = new Publisher<EquipmentActivity>(new Subscription(publisherConnString));
            Logger = new LogServiceFactory().CreateLogger<EquipmentActivitiesProcessor>();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null)
            {
                Logger.LogError("Invalid message received");
                return;
            }

            string body = message.Body;
            if (string.IsNullOrEmpty(body))
            {
                Logger.LogError("Invalid message received");
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
                        var isBookingFound = await BookingExists(equipment.BookingNumber);
                        Logger.LogInformation("Booking: {@BookingNumber} in equipment message {@Found}", equipment.BookingNumber,(equipment.BookingNumber is null ? "not found" : "exists"));
                        if (isBookingFound)
                        {
                            await Publisher.Send(equipment);
                            Logger.LogInformation("Equipment activity {@ActivityId} published", equipment.ActivityId);
                        }
                        else
                        {
                            if (await StoreEquipment(equipment))
                            {
                                Logger.LogInformation("Stored equipment activity: {@message} with booking number not found", equipment);
                            }
                            else
                            {
                                Logger.LogError("Error while storing equipment activity");
                            }
                        }
                    }
                    else
                    {
                        Logger.LogError("Equipment activity {@ActivityId} with missing booking number", equipment.ActivityId);
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
                Logger.LogError("Aborting processing for booking");
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
