using Core.DTO;
using MessagesFramework;
using Microsoft.Extensions.Logging;
using SolidPlayground_S.Processing;

// See https://aka.ms/new-console-template for more information
var loggerFactory = new Infrastructure.Logging.LogServiceFactory();
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Starting message processor");

string connectionString = Environment.GetEnvironmentVariable("connection-string") ?? "local-dev-string";
var processor = new MessageProcessor();
var bookingSub = new Subscriber<Booking>(new Subscription(connectionString), processor);
bookingSub.Subscribe();

var equipSub = new Subscriber<EquipmentActivity>(new Subscription(connectionString), processor);
equipSub.Subscribe();

Console.ReadLine();