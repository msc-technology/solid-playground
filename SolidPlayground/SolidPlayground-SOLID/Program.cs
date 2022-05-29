using Core.DTO;
using Infrastructure.Helper;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_SOLID.Processing;
using SolidPlayground_SOLID.Repository;

// logging
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();

// repositories
var bookingRepository = new BookingEventRepository();
var equipmentRepository = new EquipmentActivityEventRepository(bookingRepository);

// connection and publishing
string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";
var subscription = new Subscription(connectionString);
var publisher = new Publisher<EquipmentActivity>(subscription);

// helpers
var jsonHelper = new JsonHelper();

// processors
var bookingProcessor = new ProcessorDecorator(
    new BookingProcessor(bookingRepository, jsonHelper, logger)
    , logger);
var equipmentProcessor = new ProcessorDecorator(
    new EquipmentActivitiesProcessor(publisher, equipmentRepository, jsonHelper, logger)
    , logger);

// subscribers
var bookingSub = new Subscriber<Booking>(subscription, bookingProcessor);
var equipSub = new Subscriber<EquipmentActivity>(subscription, equipmentProcessor);

// run
logger.LogInformation("Starting message processor");
bookingSub.Subscribe();
equipSub.Subscribe();
Console.ReadLine();