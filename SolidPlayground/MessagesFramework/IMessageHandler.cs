namespace MessagesFramework
{
    public interface IMessageHandler
    {
        Task HandleMessage(Message message);
    }
}
