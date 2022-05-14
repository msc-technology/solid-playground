using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
namespace Infrastructure.Logging
{
    public class LogServiceFactory
    {
        private static Serilog.Core.Logger? innerSerilogLogger;

        public LogServiceFactory()
        {
            if (innerSerilogLogger is null)
            {
                innerSerilogLogger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger<T>()
        {
            return new SerilogLoggerFactory(innerSerilogLogger).CreateLogger<T>();
        }
    }
}
