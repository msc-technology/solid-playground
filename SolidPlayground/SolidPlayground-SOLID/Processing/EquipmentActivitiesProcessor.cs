using Core.DTO;
using Infrastructure.Helper;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_SOLID.Repository;

namespace SolidPlayground_SOLID.Processing
{
    public class EquipmentActivitiesProcessor : IMessageHandler
    {
        private readonly IPublisher<EquipmentActivity> publisher;
        private readonly IEquipmentActivityEventRepository equipmentActivityEventRepository;
        private readonly IJsonHelper jsonHelper;
        private readonly ILogger logger;

        public EquipmentActivitiesProcessor(
            IPublisher<EquipmentActivity> iPublisher,
            IEquipmentActivityEventRepository iEquipmentActivityEventRepository,
            IJsonHelper ijsonHelper,
            ILogger iLogger
        )
        {
            publisher = iPublisher;
            equipmentActivityEventRepository = iEquipmentActivityEventRepository;
            jsonHelper = ijsonHelper;
            logger = iLogger;
        }

        public async Task HandleMessage(Message message)
        {
            EquipmentActivity? equipment = jsonHelper.Deserialize<EquipmentActivity>(message.Body);
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
    }
}
