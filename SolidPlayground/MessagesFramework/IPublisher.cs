namespace MessagesFramework
{
    public interface IPublisher<T>
    {
        Task Send(string message);
        Task Send(T? message);
    }
}
