namespace shop.api;

public record Fruit(Guid Id, string Name, string Color) : DomainModel(Id);

public record CreateFruitEvent(Guid FruitId, string Name, string Color) : Event<Fruit>(FruitId)
{
    public override Fruit? Apply(Fruit? existing)
    {
        return new Fruit(FruitId, Name, Color);
    }
}

public record UpdateFruitEvent(Guid FruitId, string Color) : Event<Fruit>(FruitId)
{
    public override Fruit? Apply(Fruit? existing)
    {
        return existing is null ? null : existing with { Color = Color };
    }
}

public record DeleteFruitEvent(Guid FruitId) : Event<Fruit>(FruitId)
{
    public override Fruit? Apply(Fruit? existing)
    {
        return null;
    }
}
