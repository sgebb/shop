namespace shop.api;

public abstract record Event<T>(Guid ModelId) where T : DomainModel
{ 
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset AppliesAt { get; set; } = DateTimeOffset.Now;
    public T? ApplyTo(T? existing)
    {
        var created = existing?.CreatedAt ?? AppliesAt;
        return Apply(existing) with { CreatedAt = created, UpdatedAt = AppliesAt };

    }

    public abstract T? Apply(T? existing);
}

public abstract record DomainModel(Guid Id)
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}