using shop.eventsourcing;

namespace shop.shared;

public class DomainService<T>(IEventStore<T> _eventStore) : IDomainService<T> where T : DomainModel
{
    public IEnumerable<T> Get(DateTimeOffset? at = null) => _eventStore
        .Events()
        .GroupBy(f => f.ModelId)
        .Select(g => g.ToModel(at)!)
        .Where(f => f is not null);

    public T? Get(Guid id, DateTimeOffset? at = null) => _eventStore
        .Events(id)
        .ToModel(at);

    public IEnumerable<T?> GetHistorical(Guid id) => _eventStore
        .Events(id)
        .ToModelHistorical();

    public IEnumerable<IEnumerable<T?>> GetHistorical() => _eventStore.Events()
        .GroupBy(f => f.ModelId)
        .Select(g => g.ToModelHistorical())
        .Where(f => f is not null);

    public void AddEvent(Event<T> e) => _eventStore
        .AddEvent(e);
}
