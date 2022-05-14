﻿using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_I.Repository;
using Infrastructure.Logging;
using System.Text.Json;

namespace SolidPlayground_I.Processing
{
    public class BookingProcessor : IMessageHandler
    {
        // Violates:
        // D: Dipendency Inversion
        private readonly ILogger logger;
        private readonly BookingEventRepository BookingEventRepository;

        public BookingProcessor()
        {
            var loggerFactory = new LogServiceFactory();
            logger = loggerFactory.CreateLogger<BookingProcessor>();
            BookingEventRepository = new BookingEventRepository();
        }

        public async Task HandleMessage(Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Body))
            {
                logger.LogError("Invalid message received");
                return;
            }

            string body = message.Body;

            Booking booking = JsonSerializer.Deserialize<Booking>(body);
            if (booking is null)
            {
                logger.LogError("Invalid booking received");
            }

            if (await BookingEventRepository.Exists(booking.BookingNumber))
            {
                logger.LogInformation("Booking {@BookingNumber} was already stored", booking.BookingNumber);
                return;
            }

            if (!await BookingEventRepository.Store(booking))
            {
                logger.LogError("Error while storing Booking {@BookingNumber}", booking.BookingNumber);
            }
            else
            {
                logger.LogInformation("Stored booking: {@BookingNumber}", booking.BookingNumber);
            }
        }
    }
}