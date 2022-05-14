namespace MessagesFramework
{
    public class Publisher<T> : IPublisher<T>
    {
        Subscription subscriptionObject;

        public Publisher(
            Subscription subscription
        )
        {
            subscriptionObject = subscription;
        }

        public Task Send(string message)
        {
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
