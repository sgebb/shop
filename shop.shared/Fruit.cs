using shop.eventsourcing;

namespace shop.shared;
public record Fruit(Guid Id, string Name, string Color) : DomainModel(Id)
{
    public int Holdings { get; init; }
};

public record CreateFruitEvent(Guid FruitId, string Name, string Color, DateTimeOffset AppliesAt) : EventBase, IEvent<Fruit>
{
    public Guid ModelId { get; set; } = FruitId;
    public DateTimeOffset AppliesAt { get; set; } = AppliesAt;

    public Fruit? Apply(Fruit? existing)
    {
        return new Fruit(FruitId, Name, Color);
    }
}

public record UpdateFruitEvent(Guid FruitId, string? Color, DateTimeOffset AppliesAt) : EventBase, IEvent<Fruit>
{
    public Guid ModelId { get; set; } = FruitId;
    public DateTimeOffset AppliesAt { get; set; } = AppliesAt;

    public Fruit? Apply(Fruit? existing)
    {
        return existing is null ? null : existing with
        {
            Color = Color is null ? existing.Color : Color
        };
    }
}

public record DeleteFruitEvent(Guid FruitId, DateTimeOffset AppliesAt) : EventBase, IEvent<Fruit>
{
    public Guid ModelId { get; set; } = FruitId;
    public DateTimeOffset AppliesAt { get; set; } = AppliesAt;
    public Fruit? Apply(Fruit? existing)
    {
        return null;
    }
}

public record DepositFruitEvent(Guid FruitId, int Amount, DateTimeOffset AppliesAt) : EventBase, IEvent<Fruit>
{
    public Guid ModelId { get; set; } = FruitId;
    public DateTimeOffset AppliesAt { get; set; } = AppliesAt;
    public Fruit? Apply(Fruit? existing)
    {
        return existing is null ? null : existing with
        {
            Holdings = existing.Holdings + Amount
        };
    }
}

public record SellFruitEvent(Guid FruitId, Guid CustomerId, int Amount, DateTimeOffset AppliesAt)
    : EventBase, IEvent<Fruit>, IEvent<Customer>
{
    Guid IEvent<Customer>.ModelId { get; set; } = CustomerId;
    Guid IEvent<Fruit>.ModelId { get; set; } = FruitId;
    public DateTimeOffset AppliesAt { get; set; } = AppliesAt;

    public Fruit? Apply(Fruit? existing)
    {
        return existing is null ? null : existing with
        {
            Holdings = existing.Holdings - Amount
        };
    }

    public Customer? Apply(Customer? existing)
    {
        if (existing == null)
        {
            return null;
        }

        if(existing.FruitHoldings.TryGetValue(FruitId, out var _))
        {
            existing.FruitHoldings[FruitId] += Amount;
        }
        else
        {
            existing.FruitHoldings.Add(FruitId, Amount);
        }

        return existing;
    }
}
