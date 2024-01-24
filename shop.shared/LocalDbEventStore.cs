using Microsoft.EntityFrameworkCore;
using shop.eventsourcing;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace shop.shared;

public class LocalDbEventStore : IEventStore
{
    private readonly ShopDbContext _shopDb;

    public LocalDbEventStore(ShopDbContext shopDb)
    {
        _shopDb = shopDb;
        _shopDb.Database.Migrate();
    }

    public void AddEvent<T>(Event<T> e) where T : DomainModel
    {
        var entity = new EventEntity(e.ModelId, e.AppliesAt, e.CreatedAt, typeof(T).FullName!, e.GetType().FullName!, JsonSerializer.Serialize(e, e.GetType()));
        _shopDb.Add(entity);
        _shopDb.SaveChanges();
    }

    public IEnumerable<DomainEvent> AllEvents()
    {
        return _shopDb.Events
            .AsEnumerable()
            .Select(e => Deserialize<DomainEvent>(e));
    }

    private static T Deserialize<T>(EventEntity e) where T : class
    {
        Type t = Type.GetType(e.EventType);
        return JsonSerializer.Deserialize(e.Content, t) as T;
    }

    public IEnumerable<Event<T>> Events<T>() where T : DomainModel
    {
        return _shopDb.Events
            .Where(e => e.DomainModelType == typeof(T).FullName)
            .AsEnumerable()
            .Select(Deserialize<Event<T>>);
    }

    public IEnumerable<Event<T>> EventsFor<T>(Guid modelId) where T : DomainModel
    {
        return _shopDb.Events
            .Where(e => e.DomainModelType == typeof(T).FullName)
            .Where(e => e.ModelId == modelId)
            .AsEnumerable()
            .Select(Deserialize<Event<T>>);
    }
}

public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<EventEntity> Events { get; set; }

}

public record EventEntity(Guid ModelId, DateTimeOffset AppliesAt, DateTimeOffset CreatedAt, string DomainModelType, string EventType, string Content)
{
    [Key]
    public Guid EventId { get; set; }
};