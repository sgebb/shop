namespace shop.api.Fruits;

public record FruitPatch(string Color, DateTimeOffset AppliesAt);
public record FruitPost(string Name, string Color, DateTimeOffset AppliesAt);

