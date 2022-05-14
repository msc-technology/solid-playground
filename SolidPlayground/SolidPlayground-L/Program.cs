using Core.DTO;
using MessagesFramework;
using SolidPlayground_L.Processing;

// See https://aka.ms/new-console-template for more information
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();
Console.WriteLine("Starting message processor");

string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";
var bookingProcessor = new BookingProcessor();
var bookingSub = new Subscriber<Booking>(new Subscription(connectionString), bookingProcessor);
bookingSub.Subscribe();

var equipmentProcessor = new EquipmentActivitiesProcessor();
var equipSub = new Subscriber<EquipmentActivity>(new Subscription(connectionString), equipmentProcessor);
equipSub.Subscribe();

Console.ReadLine();