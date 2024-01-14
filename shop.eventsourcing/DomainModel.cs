namespace shop.eventsourcing;

public abstract record DomainModel(Guid Id)
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } 
}