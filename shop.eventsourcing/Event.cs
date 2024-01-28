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
    public DateTimeOffset AppliesAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public T? Apply(T? existing);

    internal T? On(T? existing)
    {
        var after = Apply(existing);
        if (after == null)
        {
            return null;
        }

        var createdAt = existing?.CreatedAt ?? AppliesAt;
        return after with { CreatedAt = createdAt, UpdatedAt = AppliesAt };
    }
}