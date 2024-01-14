using shop.eventsourcing;
using shop.shared;

namespace Microsoft.Extensions.DependencyInjection;

public static class SharedServiceCollectionExtensions
{
    public static IServiceCollection AddSharedDependencies(this IServiceCollection services)
    {
        var inMemoryFruitEventStore = new InMemoryEventStore<Fruit>(FruitEvents());

        services
            .AddSingleton<IEventStore<Fruit>>(inMemoryFruitEventStore)
            .AddTransient<IDomainService<Fruit>, DomainService<Fruit>>();

        return services;
    }

    private static List<Event<Fruit>> FruitEvents()
    {
        List<Event<Fruit>> fruitEvents = [];

        var appleId = "Apple".ToGuid();
        fruitEvents.Add(new CreateFruitEvent(appleId, "Apple", "Red"));
        fruitEvents.Add(new UpdateFruitEvent(appleId, "Apple", "Green"));
        fruitEvents.Add(new DeleteFruitEvent(appleId));

        var bananaId = "Banana".ToGuid();
        fruitEvents.Add(new CreateFruitEvent(bananaId, "Banana", "Yellow"));
        fruitEvents.Add(new UpdateFruitEvent(bananaId, "Apple", "Orange"));

        return fruitEvents;
    }
}
