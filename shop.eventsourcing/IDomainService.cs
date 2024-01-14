
using shop.eventsourcing;

namespace shop.shared
{
    public interface IDomainService<T> where T : DomainModel
    {
        void AddEvent(Event<T> e);
        IEnumerable<T> Get(DateTimeOffset? at = null);
        T? Get(Guid id, DateTimeOffset? at = null);
        IEnumerable<T?> GetHistorical(Guid id);
        IEnumerable<IEnumerable<T?>> GetHistorical();
    }
}