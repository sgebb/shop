namespace shop.eventsourcing;

public interface IEventStore<T> where T : DomainModel
{
    IEnumerable<Event<T>> Events();
    IEnumerable<Event<T>> Events(Guid modelId);
    void AddEvent(Event<T> e);
}

public class InMemoryEventStore<T>(List<Event<T>>? domainEvents = null) : IEventStore<T> where T : DomainModel
{
    private readonly List<Event<T>> _domainEvents = domainEvents ?? [];

    public void AddEvent(Event<T> e) => 
        _domainEvents.Add(e);

    public IEnumerable<Event<T>> Events() =>
        _domainEvents;

    public IEnumerable<Event<T>> Events(Guid modelId) =>
        _domainEvents
            .Where(e => e.ModelId == modelId);
}
