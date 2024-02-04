using shop.eventsourcing;

namespace shop.shared;

public interface IDomainService<T> where T : DomainModel
{
    void AddEvent(IEvent<T> e);
    IEnumerable<T> Get(DateTimeOffset? at = null);
    T? Get(Guid id, DateTimeOffset? at = null);
    IEnumerable<T> GetHistorical(Guid id);
    IEnumerable<IEnumerable<T>> GetHistorical();
}

public class DomainService<T>(IEventStore _eventStore) : IDomainService<T> where T : DomainModel
{
    public IEnumerable<T> Get(DateTimeOffset? at = null) => _eventStore
        .Events<T>()
        .GroupBy(f => f.ModelId)
        .Select(g => g.ToModel(at))
        .Where(m => m is not null)!;

    public T? Get(Guid id, DateTimeOffset? at = null) => _eventStore
        .EventsFor<T>(id)
        .ToModel(at);

    public IEnumerable<T> GetHistorical(Guid id) => _eventStore
        .EventsFor<T>(id)
        .ToModelHistorical();

    public IEnumerable<IEnumerable<T>> GetHistorical() => _eventStore
        .Events<T>()
        .GroupBy(f => f.ModelId)
        .Select(g => g.ToModelHistorical())
        .Where(f => f is not null);

    public void AddEvent(IEvent<T> e) => _eventStore
        .AddEvent(e);
}
