using shop.eventsourcing;
using shop.shared;

namespace Microsoft.Extensions.DependencyInjection;

public static class SharedServiceCollectionExtensions
{
    private static DateTimeOffset currentDateTimeOffset = DateTimeOffset.Now.AddDays(-10);

    public static IServiceCollection AddSharedDependencies(this IServiceCollection services)
    {
        var inMemoryFruitEventStore = new InMemoryEventStore(FruitEvents());

        services
            .AddSingleton<IEventStore>(inMemoryFruitEventStore)
            .AddTransient<IDomainService<Fruit>, DomainService<Fruit>>();

        return services;
    }

    private static List<DomainEvent> FruitEvents()
    {
        List<DomainEvent> fruitEvents = [];

        var appleId = "Apple".ToGuid();
        fruitEvents.Add(new CreateFruitEvent(appleId, "Apple", "Red", EventDate()));
        fruitEvents.Add(new UpdateFruitEvent(appleId, "Green", EventDate()));
        fruitEvents.Add(new DeleteFruitEvent(appleId, EventDate()));

        var bananaId = "Banana".ToGuid();
        fruitEvents.Add(new CreateFruitEvent(bananaId, "Banana", "Yellow", EventDate()));
        fruitEvents.Add(new UpdateFruitEvent(bananaId, "Orange", EventDate()));


        var kiwiId = "Kiwi".ToGuid();
        fruitEvents.Add(new CreateFruitEvent(kiwiId, "Kiwi", "Yellow", EventDate()));
        fruitEvents.Add(new UpdateFruitEvent(kiwiId, "Orange", EventDate()));

        return fruitEvents;
    }

    private static DateTimeOffset EventDate()
    {
        // Adding one day to the input DateTimeOffset
        currentDateTimeOffset = currentDateTimeOffset.AddDays(1);

        return currentDateTimeOffset;
    }
}
