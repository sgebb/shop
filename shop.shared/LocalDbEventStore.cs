using Microsoft.EntityFrameworkCore;
using shop.eventsourcing;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;

namespace shop.shared;

public class LocalDbEventStore : IEventStore
{
    private readonly ShopDbContext _shopDb;

    public LocalDbEventStore(ShopDbContext shopDb)
    {
        _shopDb = shopDb;

        var assemblyName = Assembly.GetEntryAssembly()!.GetName().Name;
        if (assemblyName != "shop.web")
        {
            _shopDb.Database.Migrate();
        }
    }

    private static T Deserialize<T>(IEventEntity e) where T : class
    {
        Type t = Type.GetType(e.EventType)!;
        return (JsonSerializer.Deserialize(e.Content, t) as T)!;
    }

    IEnumerable<IEvent<T>> IEventStore.Events<T>()
    {
        return _shopDb.IEvents
            .Where(e => e.DomainModelType.Contains(typeof(T).FullName!))
            .AsEnumerable()
            .Select(Deserialize<IEvent<T>>);
    }

    IEnumerable<IEvent<T>> IEventStore.EventsFor<T>(Guid modelId)
    {
        return _shopDb.IEvents
            .Where(e => e.DomainModelType.Contains(typeof(T).FullName!))
            .Where(e => e.ModelId.Contains(modelId))
            .AsEnumerable()
            .Select(Deserialize<IEvent<T>>);
    }

    public void AddEvent<T>(IEvent<T> e) where T : DomainModel
    {
        Type eventType = e.GetType();

        List<Guid> modelIds = [];
        List<string> domainModelTypes = [];

        foreach (Type interfaceType in eventType.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEvent<>))
            {
                Type modelType = interfaceType.GetGenericArguments()[0];
                domainModelTypes.Add(modelType.FullName!);
                PropertyInfo modelIdProperty = interfaceType.GetProperty("ModelId")!;
                if (modelIdProperty != null)
                {
                    Guid modelId = (Guid)modelIdProperty.GetValue(e)!;
                    modelIds.Add(modelId);
                }
            }
        }

        var entity = new IEventEntity(
            modelIds,
            e.AppliesAt, 
            e.CreatedAt, 
            domainModelTypes, 
            eventType.FullName!,
            JsonSerializer.Serialize(e, eventType));
        _shopDb.Add(entity);
        _shopDb.SaveChanges();
    }
}

public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<IEventEntity> IEvents { get; set; }

}

public record IEventEntity(
    List<Guid> ModelId,
    DateTimeOffset AppliesAt,
    DateTimeOffset CreatedAt, 
    List<string> DomainModelType,
    string EventType, 
    string Content)
{
    [Key]
    public Guid EventId { get; set; }
}