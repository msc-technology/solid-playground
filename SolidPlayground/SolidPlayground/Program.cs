// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;

var loggerFactory = new SolidPlayground.Infrastructure.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Starting message processor");
Console.ReadLine();
