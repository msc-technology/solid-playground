using MessagesFramework.MessageGenerators;

namespace MessagesFramework
{
    public class Subscriber<T>
    {
        private readonly IMessageGenerator Generator;
        private readonly Timer timer;
        private readonly IMessageHandler Handler;

        public Subscriber(
            Subscription subscription,
            IMessageHandler handler
        )
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Generator = new MessageGenerator<T>();
            timer = new Timer(new TimerCallback(SendMessage), () => Generator.Generate(), new TimeSpan(1000), new TimeSpan(800));
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
            await Handler.HandleMessage(message);
        }
    }
}
