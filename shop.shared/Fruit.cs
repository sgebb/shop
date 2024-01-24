using shop.eventsourcing;

namespace shop.shared;
public record Fruit(Guid Id, string Name, string Color) : DomainModel(Id);

public record CreateFruitEvent(Guid FruitId, string Name, string Color, DateTimeOffset AppliesAt) : Event<Fruit>(FruitId, AppliesAt)
{
    public override Fruit? Apply(Fruit? existing)
    {
        return new Fruit(FruitId, Name, Color);
    }
}

public record UpdateFruitEvent(Guid FruitId, string? Color, DateTimeOffset AppliesAt) : Event<Fruit>(FruitId, AppliesAt)
{
    public override Fruit? Apply(Fruit? existing)
    {
        return existing is null ? null : existing with
        {
            Color = Color is null ? existing.Color : Color
        };
    }
}

public record DeleteFruitEvent(Guid FruitId, DateTimeOffset AppliesAt) : Event<Fruit>(FruitId, AppliesAt)
{
    public override Fruit? Apply(Fruit? existing)
    {
        return null;
    }
}
