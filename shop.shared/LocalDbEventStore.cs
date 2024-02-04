using Microsoft.EntityFrameworkCore;
using shop.eventsourcing;
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
            .OrderBy(e => e.CreatedAt)
            .Where(e => e.DomainModelType.Contains(typeof(T).FullName!))
            .AsEnumerable()
            .Select(Deserialize<IEvent<T>>);
    }

    IEnumerable<IEvent<T>> IEventStore.EventsFor<T>(Guid modelId)
    {
        return _shopDb.IEvents
            .OrderBy(e => e.CreatedAt)
            .Where(e => e.DomainModelType.Contains(typeof(T).FullName!))
            .Where(e => e.ModelId.Contains(modelId))
            .AsEnumerable()
            .Select(Deserialize<IEvent<T>>);
    }

    public void AddEvent(IEventBase e)
    {
        Type eventType = e.GetType();

        var (domainModelTypes, modelIds) = e.GetTypeAndModelData();

        var entity = new IEventEntity(
            modelIds, 
            e.CreatedAt, 
            domainModelTypes.Select(t => t.FullName!).ToList(), 
            eventType.FullName!,
            JsonSerializer.Serialize(e, eventType));
        _shopDb.Add(entity);
        _shopDb.SaveChanges();
    }
}