using shop.eventsourcing;

namespace shop.shared;

public class InMemoryEventStore(List<DomainEvent>? domainEvents = null) : IEventStore
{
    private readonly List<DomainEvent> _domainEvents = domainEvents ?? [];

    public IEnumerable<DomainEvent> AllEvents()
        => _domainEvents;

    public IEnumerable<Event<T>> Events<T>() where T : DomainModel =>
        _domainEvents
            .Where(e => e is Event<T>)
            .Select(e => (e as Event<T>)!);

    public IEnumerable<Event<T>> EventsFor<T>(Guid modelId) where T : DomainModel =>
        Events<T>()
            .Where(e => e.ModelId == modelId)
            .OrderByDescending(e => e.AppliesAt);

    public void AddEvent<T>(Event<T> e) where T : DomainModel =>
        _domainEvents.Add(e);
}
