using Core.DTO;
using Infrastructure.Helper;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_D.Processing;
using SolidPlayground_D.Repository;

// logging
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();

// repositories
var bookingRepository = new BookingEventRepository();
var equipmentRepository = new EquipmentActivityEventRepository();

// connection and publishing
string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";
var subscription = new Subscription(connectionString);
var publisher = new Publisher<EquipmentActivity>(subscription);

// helpers
var jsonHelper = new JsonHelper();

// processors
var processor = new MessageProcessor(publisher, bookingRepository, equipmentRepository, jsonHelper, logger);

// subscribers
var equipSub = new Subscriber<EquipmentActivity>(subscription, processor);
var bookingSub = new Subscriber<Booking>(subscription, processor);

// run
logger.LogInformation("Starting message processor");
equipSub.Subscribe();
bookingSub.Subscribe();
Console.ReadLine();