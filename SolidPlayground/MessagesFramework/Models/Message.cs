namespace MessagesFramework
{
    public class Message
    {
        public string Body { get; private set; }

        public Message(
            string body    
        )
        {
            Body = body;
        }
    }
}
