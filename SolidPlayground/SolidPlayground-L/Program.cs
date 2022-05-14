using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_L.Processing;

// logging
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();

// connection
string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";
var subscription = new Subscription(connectionString);

// processors
var bookingProcessor = new BookingProcessor();
var equipmentProcessor = new EquipmentActivitiesProcessor();

// subscribers
var bookingSub = new Subscriber<Booking>(subscription, bookingProcessor);
var equipSub = new Subscriber<EquipmentActivity>(subscription, equipmentProcessor);

// run
logger.LogInformation("Starting message processor");
bookingSub.Subscribe();
equipSub.Subscribe();
Console.ReadLine();