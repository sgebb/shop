namespace shop.eventsourcing;

public abstract record EventBase
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
}

public interface IEvent<T> where T : DomainModel
{
    public Guid EventId { get; set; }
    public Guid ModelId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public T Apply(T? existing);

    internal T On(T? existing)
    {
        var after = Apply(existing);

        var createdAt = existing?.CreatedAt ?? CreatedAt;
        return after with { CreatedAt = createdAt, UpdatedAt = CreatedAt };
    }
}