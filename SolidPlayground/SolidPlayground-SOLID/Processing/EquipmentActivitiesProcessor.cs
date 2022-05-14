using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_SOLID.Repository;
using System.Text.Json;

namespace SolidPlayground_SOLID.Processing
{
    public class EquipmentActivitiesProcessor : IMessageHandler
    {
        private readonly IPublisher<EquipmentActivity> publisher;
        private readonly ILogger logger;
        private readonly IEquipmentActivityEventRepository equipmentActivityEventRepository;

        public EquipmentActivitiesProcessor(
            IPublisher<EquipmentActivity> iPublisher,
            IEquipmentActivityEventRepository iEquipmentActivityEventRepository,
            ILogger iLogger
        )
        {
            publisher = iPublisher;
            logger = iLogger;
            equipmentActivityEventRepository = iEquipmentActivityEventRepository;
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
                        var isBookingFound = await equipmentActivityEventRepository.BookingExists(equipment.BookingNumber);
                        logger.LogInformation("Booking: {@BookingNumber} in equipment message {@Found}", equipment.BookingNumber,(equipment.BookingNumber is null ? "not found" : "exists"));
                        if (isBookingFound)
                        {
                            await publisher.Send(equipment);
                            logger.LogInformation("Equipment activity {@ActivityId} published", equipment.ActivityId);
                        }
                        else
                        {
                            if (await equipmentActivityEventRepository.Store(equipment))
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
    }
}
