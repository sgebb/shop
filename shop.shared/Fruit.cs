using shop.eventsourcing;

namespace shop.shared;
public record Fruit(Guid Id, string Name, string Color) : DomainModel(Id)
{
    public int Holdings { get; init; }
};

public record CreateFruitEvent(Guid FruitId, string Name, string Color) : EventBase, IEvent<Fruit>
{
    public Guid ModelId { get; set; } = FruitId;

    public Fruit Apply(Fruit? existing)
    {
        return new Fruit(FruitId, Name, Color);
    }
}

public record UpdateFruitEvent(Guid FruitId, string? Color) : EventBase, IEvent<Fruit>
{
    public Guid ModelId { get; set; } = FruitId;

    public Fruit Apply(Fruit? existing)
    {
        return existing! with
        {
            Color = Color is null ? existing.Color : Color
        };
    }
}

public record DeleteFruitEvent(Guid FruitId) : EventBase, IEvent<Fruit>
{
    public Guid ModelId { get; set; } = FruitId;
    public Fruit Apply(Fruit? existing)
    {
        return existing! with { State = State.Deleted };
    }
}

public record DepositFruitEvent(Guid FruitId, int Amount) : EventBase, IEvent<Fruit>
{
    public Guid ModelId { get; set; } = FruitId;
    public Fruit Apply(Fruit? existing)
    {
        return existing! with
        {
            Holdings = existing.Holdings + Amount
        };
    }
}

public record SellFruitEvent(Guid FruitId, Guid CustomerId, int Amount)
    : EventBase, IEvent<Fruit>, IEvent<Customer>
{
    Guid IEvent<Customer>.ModelId { get; set; } = CustomerId;
    Guid IEvent<Fruit>.ModelId { get; set; } = FruitId;

    public Fruit Apply(Fruit? existing)
    {
        return existing! with
        {
            Holdings = existing.Holdings - Amount
        };
    }

    public Customer Apply(Customer? existing)
    {
        int sum = Amount;
        if(existing!.Holdings.TryGetValue(FruitId, out var holdings))
        {
            sum += holdings;
        }
        existing.Holdings[FruitId] = sum;
        return existing;
    }
}
