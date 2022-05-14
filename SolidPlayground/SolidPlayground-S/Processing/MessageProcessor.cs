using MessagesFramework;
using Core.DTO;
using SolidPlayground_S.Storage;
using System.Text.Json;
using SolidPlaygroundCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace SolidPlayground_S.Processing
{
    public class MessageProcessor : IMessageHandler
    {
        // Violates:
        // O: Open for change, closed to modification
        // D: Dipendency Inversion
        private readonly Publisher<EquipmentActivity> Publisher;
        private readonly ILogger Logger;
        private readonly BookingEventRepository bookingEventStorage;
        private readonly EquipmentActivityEventRepository equipmentActivityEventStorage;

        public MessageProcessor()
        {
            var publisherConnString = Environment.GetEnvironmentVariable("pub-connection-string") ?? "local-dev-string";
            Publisher = new Publisher<EquipmentActivity>(new Subscription(publisherConnString));
            Logger = new LogServiceFactory().CreateLogger<MessageProcessor>();
            bookingEventStorage = new BookingEventRepository();
            equipmentActivityEventStorage = new EquipmentActivityEventRepository();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Body))
            {
                Logger.LogError("Invalid message received");
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
                        var isBookingFound = await bookingEventStorage.Exists(equipment.BookingNumber);
                        if (isBookingFound)
                        {
                            await Publisher.Send(equipment);
                            Logger.LogInformation("Equipment activity {@ActivityId} published", equipment.ActivityId);
                        }
                        else
                        {
                            await equipmentActivityEventStorage.Store(equipment);
                            Logger.LogError("Equipment activity {@ActivityId} with booking number {@BookingNumber} not found", equipment.ActivityId, equipment.BookingNumber);
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
                Booking booking = JsonSerializer.Deserialize<Booking>(body);
                if (booking is not null)
                {
                    await bookingEventStorage.Store(booking);
                }
                else
                {
                    Logger.LogError("Invalid booking received");
                }
            }
            // error
            else
            {
                Logger.LogError("Aborting processing for booking");
            }
        }
    }
}
