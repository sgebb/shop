using Microsoft.EntityFrameworkCore;
using shop.eventsourcing;
using System.Reflection;
using System.Text.Json;

namespace shop.shared;

public class LocalDbEventStore : IEventStore
{
    private readonly ShopDbContext _shopDb;
    private readonly IEventBus _eventBus;

    public LocalDbEventStore(ShopDbContext shopDb, IEventBus eventBus)
    {
        _shopDb = shopDb;

        var assemblyName = Assembly.GetEntryAssembly()!.GetName().Name;
        if (assemblyName != "shop.web")
        {
            _shopDb.Database.Migrate();
        }
        _eventBus = eventBus;
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
        List<Type> domainModelTypes = [];

        foreach (Type interfaceType in eventType.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEvent<>))
            {
                Type modelType = interfaceType.GetGenericArguments()[0];
                domainModelTypes.Add(modelType);
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
            e.CreatedAt, 
            domainModelTypes.Select(t => t.FullName!).ToList(), 
            eventType.FullName!,
            JsonSerializer.Serialize(e, eventType));
        _shopDb.Add(entity);
        _eventBus.Publish(new RefreshEvent(domainModelTypes, modelIds));
        _shopDb.SaveChanges();
    }
}