namespace shop.api;

public interface IEventStore
{
    IEnumerable<Event<T>> Events<T>() where T : DomainModel;
    void AddEvent<T>(Event<T> e) where T : DomainModel;
    IEnumerable<Event<T>> Events<T>(Guid modelId) where T : DomainModel;
    IEnumerable<DomainEvent> AllEvents();
}

public class InMemoryEventStore : IEventStore
{
    private readonly List<DomainEvent> _domainEvents;

    public InMemoryEventStore()
    {
        _domainEvents = [];

        var appleId = "Apple".ToGuid();
        _domainEvents.Add(new CreateFruitEvent(appleId, "Apple", "Red"));
        _domainEvents.Add(new UpdateFruitEvent(appleId, "Apple", "Green"));
        _domainEvents.Add(new DeleteFruitEvent(appleId));

        var bananaId = "Banana".ToGuid();
        _domainEvents.Add(new CreateFruitEvent(bananaId, "Banana", "Yellow"));
        _domainEvents.Add(new UpdateFruitEvent(bananaId, "Apple", "Orange"));

    }

    public void AddEvent<T>(Event<T> e) where T : DomainModel => 
        _domainEvents.Add(e);

    public IEnumerable<DomainEvent> AllEvents() =>
        _domainEvents;

    public IEnumerable<Event<T>> Events<T>() where T : DomainModel =>
        _domainEvents
            .Where(e => e is Event<T>)
            .Select(e => e as Event<T>);

    public IEnumerable<Event<T>> Events<T>(Guid modelId) where T : DomainModel =>
        _domainEvents
            .Where(e => e is Event<T>)
            .Where(e => e.ModelId == modelId)
            .Select(e => e as Event<T>);
}
