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
            try { 
                EquipmentActivity? equipment = JsonSerializer.Deserialize<EquipmentActivity>(message.Body);
                var bookingExists = await equipmentActivityEventRepository.BookingExists(equipment.BookingNumber);
                logger.LogInformation("Booking: {@BookingNumber} in equipment message {@Found}", equipment.BookingNumber,(bookingExists ? "found" : "not found"));
                if (!bookingExists)
                {
                    await equipmentActivityEventRepository.Store(equipment);
                    logger.LogInformation("Stored equipment activity: {@message} with booking number not found", equipment);
                    return;
                }

                await publisher.Send(equipment);
                logger.LogInformation("Equipment activity {@ActivityId} published", equipment.ActivityId);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogError("Invalid argument {@ex}", ex);
            }
            catch (JsonException ex)
            {
                logger.LogError("Invalid json {@ex}", ex);
            }
        }
    }
}
