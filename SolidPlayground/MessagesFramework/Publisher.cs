namespace MessagesFramework
{
    public class Publisher<T>
    {
        Subscription Subscription;

        public Publisher(
            Subscription subscription
        )
        {
            Subscription = subscription;
        }

        public Task Send(string message)
        {
            Console.WriteLine($"Sent message: {message}");
            return Task.CompletedTask;
        }

        public Task Send(T? message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return Send(message.ToString());
        }
    }
}
