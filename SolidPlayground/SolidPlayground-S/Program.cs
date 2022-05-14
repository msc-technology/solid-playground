using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_S.Processing;

// logging
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();

// connection
string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";
var subscription = new Subscription(connectionString);

// subscribers
var processor = new MessageProcessor();

// subscribers
var bookingSub = new Subscriber<Booking>(subscription, processor);
var equipSub = new Subscriber<EquipmentActivity>(subscription, processor);

// run
logger.LogInformation("Starting message processor");
bookingSub.Subscribe();
equipSub.Subscribe();
Console.ReadLine();