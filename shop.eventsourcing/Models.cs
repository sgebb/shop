namespace shop.eventsourcing;

public abstract record DomainEvent(Guid ModelId);

public abstract record Event<T>(Guid ModelId) : DomainEvent(ModelId) where T : DomainModel
{ 
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset AppliesAt { get; set; } = DateTimeOffset.Now;
    internal T? On(T? existing)
    {
        var after = Apply(existing);
        if (after == null)
        {
            return null;
        }

        var createdAt = existing?.CreatedAt ?? AppliesAt;
        return Apply(existing) with { CreatedAt = createdAt, UpdatedAt = AppliesAt };

    }

    public abstract T? Apply(T? existing);
}

public abstract record DomainModel(Guid Id)
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } 
}