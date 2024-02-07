using shop.eventsourcing;
using System.Text.Json;

namespace shop.shared;

public class QueryService<T>(
    ShopDbContext _shopDbContext)
    : IQueryService<T> where T : DomainModel
{
    public IEnumerable<T> Get(DateTimeOffset? at = null)
    {
        var historical = GetHistorical();
        return historical.Select(h =>
            h.LastOrDefault(t => (at ?? DateTimeOffset.MaxValue) >= t.UpdatedAt))
            .Where(m => m is not null)!;
    }

    public T? Get(Guid id, DateTimeOffset? at = null)
    {
        var historical = GetHistorical(id);
        return historical
            .LastOrDefault(t => (at ?? DateTimeOffset.MaxValue) >= t.UpdatedAt);
    }

    public IEnumerable<T> GetHistorical(Guid id) =>
        _shopDbContext.ReadModels
            .Where(rm => rm.DomainModelType == typeof(T).FullName!)
            .Where(rm => rm.Id == id)
            .OrderBy(rm => rm.At)
            .ToList()
            .Select(rm => JsonSerializer.Deserialize<T>(rm.Content)!);

    public IEnumerable<IEnumerable<T>> GetHistorical() =>
        _shopDbContext.ReadModels
            .Where(rm => rm.DomainModelType == typeof(T).FullName!)
            .OrderBy(rm => rm.At)
            .GroupBy(rm => rm.Id)
            .ToList()
            .Select(g => g.Select(rm => JsonSerializer.Deserialize<T>(rm.Content)!));
}