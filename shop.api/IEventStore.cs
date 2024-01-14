namespace shop.api;

public interface IEventStore
{
    IEnumerable<Event<T>> Events<T>() where T : DomainModel;
    void AddEvent<T>(Event<T> e) where T : DomainModel;
    IEnumerable<Event<T>> Events<T>(Guid modelId) where T : DomainModel;
    IEnumerable<DomainEvent> AllEvents();
}
