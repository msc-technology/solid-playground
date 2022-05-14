using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_SOLID.Processing;
using SolidPlayground_SOLID.Repository;

// See https://aka.ms/new-console-template for more information
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Starting message processor");

// repositories
var bookingRepository = new BookingEventRepository();
var bookingReadonlyRepository = new BookingEventReadonlyRepository();
var equipmentRepository = new EquipmentActivityEventRepository(bookingReadonlyRepository);

// publishing
string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";
var subscription = new Subscription(connectionString);
var publisher = new Publisher<EquipmentActivity>(subscription);

// booking processor
var bookingProcessor = new BookingProcessor(bookingRepository, logger);
var bookingSub = new Subscriber<Booking>(subscription, bookingProcessor);
bookingSub.Subscribe();

// equipment processor
var equipmentProcessor = new EquipmentActivitiesProcessor(publisher, equipmentRepository, logger);
var equipSub = new Subscriber<EquipmentActivity>(subscription, equipmentProcessor);
equipSub.Subscribe();

Console.ReadLine();