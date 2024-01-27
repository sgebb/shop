namespace shop.eventsourcing;
public abstract record DomainEvent(Guid EventId, Guid ModelId, DateTimeOffset AppliesAt, DateTimeOffset CreatedAt);

public abstract record Event<T>(Guid ModelId, DateTimeOffset AppliesAt)
    : DomainEvent(EventId: Guid.NewGuid(), ModelId: ModelId, AppliesAt: AppliesAt, CreatedAt: DateTimeOffset.Now)
    where T : DomainModel
{
    internal T? On(T? existing)
    {
        var after = Apply(existing);
        if (after == null)
        {
            return null;
        }

        var createdAt = existing?.CreatedAt ?? AppliesAt;
        return Apply(existing)! with { CreatedAt = createdAt, UpdatedAt = AppliesAt };
    }

    public abstract T? Apply(T? existing);
}
