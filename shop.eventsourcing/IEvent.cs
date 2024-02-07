
using System.Reflection;

namespace shop.eventsourcing;

public abstract record EventBase : IEventBase
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
}

public interface IEventBase
{
    public Guid EventId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    (List<Type> domainModelTypes, List<Guid> modelIds) GetTypeAndModelData()
    {
        Type eventType = GetType();
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
                    Guid modelId = (Guid)modelIdProperty.GetValue(this)!;
                    modelIds.Add(modelId);
                }
            }
        }
        return (domainModelTypes, modelIds);
    }
}

public interface IEvent<T> : IEventBase where T : DomainModel
{
    public Guid ModelId { get; set; }
    public T Apply(T? existing);

    internal T On(T? existing)
    {
        var after = Apply(existing);

        var createdAt = existing?.CreatedAt ?? CreatedAt;
        return after with { CreatedAt = createdAt, UpdatedAt = CreatedAt };
    }
}