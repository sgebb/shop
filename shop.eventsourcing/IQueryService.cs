namespace shop.eventsourcing;

public interface IQueryService<T> where T : DomainModel
{
    IEnumerable<T> Get(DateTimeOffset? at = null);
    T? Get(Guid id, DateTimeOffset? at = null);
    IEnumerable<T> GetHistorical(Guid id);
    IEnumerable<IEnumerable<T>> GetHistorical();
}
