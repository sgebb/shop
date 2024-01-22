namespace shop.eventsourcing;

public interface IEventStore
{
    IEnumerable<DomainEvent> AllEvents();
    IEnumerable<Event<T>> Events<T>() where T : DomainModel;
    IEnumerable<Event<T>> EventsFor<T>(Guid modelId) where T : DomainModel;
    void AddEvent<T>(Event<T> e) where T : DomainModel;
}
