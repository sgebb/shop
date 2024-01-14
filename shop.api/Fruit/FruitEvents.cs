using shop.eventsourcing;
using shop.shared;

namespace shop.api;

public record CreateFruitEvent(Guid FruitId, string Name, string Color) : Event<Fruit>(FruitId)
{
    public override Fruit? Apply(Fruit? existing)
    {
        return new Fruit(FruitId, Name, Color);
    }
}

public record UpdateFruitEvent(Guid FruitId, string Name, string Color) : Event<Fruit>(FruitId)
{
    public override Fruit? Apply(Fruit? existing)
    {
        return existing is null ? null : existing with { Name = Name, Color = Color };
    }
}

public record DeleteFruitEvent(Guid FruitId) : Event<Fruit>(FruitId)
{
    public override Fruit? Apply(Fruit? existing)
    {
        return null;
    }
}
