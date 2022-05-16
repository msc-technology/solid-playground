using MessagesFramework;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SolidPlayground_SOLID.Processing
{
    public class ProcessorDecorator : IMessageHandler
    {
        private readonly IMessageHandler processor;
        private readonly ILogger logger;

        public ProcessorDecorator(
            IMessageHandler messageHandler,
            ILogger iLogger
        )
        {
            processor = messageHandler;
            logger = iLogger;
        }

        public async Task HandleMessage(Message message)
        {
            try
            {
                await processor.HandleMessage(message);
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
