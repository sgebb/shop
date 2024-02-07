namespace shop.eventsourcing;

public interface IEventBus
{
    IAsyncEnumerable<T> Subscribe<T>();
    Task PublishAsync<T>(T e);
}
