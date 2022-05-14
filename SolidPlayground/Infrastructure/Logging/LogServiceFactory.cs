using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
namespace Infrastructure.Logging
{
    public class LogServiceFactory
    {
        private static Serilog.Core.Logger? InnerSerilogLogger;

        public LogServiceFactory()
        {
            if (InnerSerilogLogger is null)
            {
                InnerSerilogLogger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger<T>()
        {
            return new SerilogLoggerFactory(InnerSerilogLogger).CreateLogger<T>();
        }
    }
}
