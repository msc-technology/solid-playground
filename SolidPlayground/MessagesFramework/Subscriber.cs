using MessagesFramework.MessageGenerators;

namespace MessagesFramework
{
    public class Subscriber<T>
    {
        private readonly IMessageGenerator generator;
        private readonly Timer timer;
        private readonly IMessageHandler messageHandler;

        public Subscriber(
            Subscription subscription,
            IMessageHandler handler
        )
        {
            messageHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            generator = new MessageGenerator<T>();
            timer = new Timer(new TimerCallback(SendMessage), () => generator.Generate(), new TimeSpan(1000), new TimeSpan(800));
        }

        public void Subscribe()
        {
            var rnd = new Random();
            var interval = rnd.NextInt64(400, 1200);
            timer.Change(800, interval);
        }

        private async void SendMessage(object obj)
        {
            var func = (Func<Message>)obj;
            Message message = func.Invoke();
            await messageHandler.HandleMessage(message);
        }
    }
}
