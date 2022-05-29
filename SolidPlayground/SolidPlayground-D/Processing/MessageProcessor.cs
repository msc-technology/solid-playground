using Core.DTO;
using Infrastructure.Helper;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_D.Repository;

namespace SolidPlayground_D.Processing
{
    public class MessageProcessor : IMessageHandler
    {
        // Violates:
        // O: Open for change, closed to modification
        private readonly IPublisher<EquipmentActivity> publisher;
        private readonly IRepository<Booking?> bookingEventRepository;
        private readonly IRepository<EquipmentActivity?> equipmentActivityEventStorage;
        private readonly IJsonHelper jsonHelper;
        private readonly ILogger logger;

        public MessageProcessor(
            IPublisher<EquipmentActivity> iPublisher,
            IRepository<Booking?> iBookingRepository,
            IRepository<EquipmentActivity?> iEquipmentActivityRepository,
            IJsonHelper ijsonHelper,
            ILogger iLogger
        )
        {
            publisher = iPublisher;
            bookingEventRepository = iBookingRepository;
            equipmentActivityEventStorage = iEquipmentActivityRepository;
            jsonHelper = ijsonHelper;
            logger = iLogger;
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Body))
            {
                logger.LogError("Invalid message received");
                return;
            }

            // equipment activities
            if (message.Body.Contains("ActivityId"))
            {
                EquipmentActivity? equipment = jsonHelper.Deserialize<EquipmentActivity>(message.Body);
                if (equipment is not null)
                {
                    if (!string.IsNullOrWhiteSpace(equipment.BookingNumber))
                    {
                        var isBookingFound = await bookingEventRepository.Exists(equipment.BookingNumber);
                        if (isBookingFound)
                        {
                            await publisher.Send(equipment);
                            logger.LogInformation("Equipment activity {@ActivityId} published", equipment.ActivityId);
                        }
                        else
                        {
                            await equipmentActivityEventStorage.Store(equipment);
                            logger.LogError("Equipment activity {@ActivityId} with booking number {@BookingNumber} not found", equipment.ActivityId, equipment.BookingNumber);
                        }
                    }
                    else
                    {
                        logger.LogError("Equipment activity {@ActivityId} with missing booking number", equipment.ActivityId);
                    }
                }
            }
            // booking
            else if (message.Body.Contains("BookingNumber"))
            {
                Booking? booking = jsonHelper.Deserialize<Booking>(message.Body);
                if (booking is null)
                {
                    logger.LogError("Invalid booking received");
                    return;
                }

                if (await bookingEventRepository.Exists(booking.BookingNumber))
                {
                    logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                    return;
                }

                await bookingEventRepository.Store(booking);
                logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
            }
            // error
            else
            {
                logger.LogError("Aborting processing for booking");
            }
        }
    }
}
