using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_SpaghettiCode.Processing;

// logging
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();

// connection
string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";

// processors
var processor = new MessageProcessor();

// subscribers
var bookingSub = new Subscriber<Booking>(new Subscription(connectionString), processor);
var equipSub = new Subscriber<EquipmentActivity>(new Subscription(connectionString), processor);

// run
logger.LogInformation("Starting message processor");
bookingSub.Subscribe();
equipSub.Subscribe();
Console.ReadLine();