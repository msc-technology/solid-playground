using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_SOLID.Processing;
using SolidPlayground_SOLID.Repository;

// logging
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();

// repositories
var bookingRepository = new BookingEventRepository();
var bookingReadonlyRepository = new BookingEventReadonlyRepository();
var equipmentRepository = new EquipmentActivityEventRepository(bookingReadonlyRepository);

// connection and publishing
string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";
var subscription = new Subscription(connectionString);
var publisher = new Publisher<EquipmentActivity>(subscription);

// processors
var bookingProcessor = new BookingProcessor(bookingRepository, logger);
var equipmentProcessor = new EquipmentActivitiesProcessor(publisher, equipmentRepository, logger);

// subscribers
var bookingSub = new Subscriber<Booking>(subscription, bookingProcessor);
var equipSub = new Subscriber<EquipmentActivity>(subscription, equipmentProcessor);

// run
logger.LogInformation("Starting message processor");
bookingSub.Subscribe();
equipSub.Subscribe();
Console.ReadLine();