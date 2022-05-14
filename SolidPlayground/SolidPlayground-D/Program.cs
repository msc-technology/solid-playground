using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_D.Processing;
using SolidPlayground_D.Repository;

// See https://aka.ms/new-console-template for more information
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Starting message processor");

// repositories
var bookingRepository = new BookingEventRepository();
var equipmentRepository = new EquipmentActivityEventRepository();

// publishing
string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";
var subscription = new Subscription(connectionString);
var publisher = new Publisher<EquipmentActivity>(subscription);

var processor = new MessageProcessor(publisher, bookingRepository, equipmentRepository, logger);
var bookingSub = new Subscriber<Booking>(subscription, processor);
bookingSub.Subscribe();

var equipSub = new Subscriber<EquipmentActivity>(subscription, processor);
equipSub.Subscribe();

Console.ReadLine();