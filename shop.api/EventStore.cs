namespace shop.api;

public static class EventStore
{
    public static IEnumerable<Event<Fruit>> FruitEvents()
    {
        var events = new List<Event<Fruit>>();

        var appleId = "Apple".ToGuid();
        events.AddRange(new List<Event<Fruit>>() {
            new CreateFruitEvent(appleId, "Apple", "Red"),
            new UpdateFruitEvent(appleId, "Green"),
            new DeleteFruitEvent(appleId) });

        var bananaId = "Banana".ToGuid();
        events.AddRange(new List<Event<Fruit>>() {
            new CreateFruitEvent(bananaId, "Banana", "Yellow"),
            new UpdateFruitEvent(bananaId, "Orange"),
            new DeleteFruitEvent(bananaId) });

        return events;
    }
}
