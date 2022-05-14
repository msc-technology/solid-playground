using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;
using System.Text.Json;
using Infrastructure.Logging;

namespace SolidPlayground_SpaghettiCode.Processing
{
    public class MessageProcessor : IMessageHandler
    {
        // Violates:
        // S: Single responsability
        // O: Open for change, closed to modification
        // D: Dipendency Inversion
        private readonly Publisher<EquipmentActivity> publisher;
        private readonly ILogger logger;

        public MessageProcessor()
        {
            var publisherConnString = Environment.GetEnvironmentVariable("pub-connection-string") ?? "local-dev-string";
            publisher = new Publisher<EquipmentActivity>(new Subscription(publisherConnString));
            var loggerFactory = new LogServiceFactory();
            logger = loggerFactory.CreateLogger<MessageProcessor>();
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
                        var isBookingFound = await BookingExists(equipment.BookingNumber);
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
                Booking booking = JsonSerializer.Deserialize<Booking>(body);
                if (booking is null)
                {
                    logger.LogError("Invalid booking received");
                }

                if (await BookingExists(booking.BookingNumber))
                {
                    logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                    return;
                }

                if (!await StoreBooking(booking))
                {
                    logger.LogError("Error while storing Booking {@BookingNumber}", booking.BookingNumber);
                }
                else
                {
                    logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
                }
            }
            // error
            else
            {
                logger.LogError("Aborting processing for booking");
            }
        }

        // db methods
        private async Task<bool> StoreBooking(Booking message)
        {
            using (var db = new StorageContext())
            {
                if (message is null)
                {
                    return false;
                }

                await db.BookingEntity.AddAsync(new BookingEntity(message.BookingNumber));
                db.SaveChanges();
                return true;
            }
        }

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
