namespace MessagesFramework
{
    public class Subscription
    {
        public string ConnectionString { get; private set; }

        public Subscription(
            string connectionString
        )
        {
            ConnectionString = connectionString;
        }
    }
}
