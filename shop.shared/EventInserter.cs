using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using shop.eventsourcing;

namespace shop.shared;

public class EventInserter(
    IEventBus _bus,
    IServiceScopeFactory _scopeFactory)
    : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var events = _bus.Subscribe<IEventBase>();
        await foreach (var e in events)
        {
            using var scope = _scopeFactory.CreateScope();
            var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();

            eventStore.AddEvent(e);

            var (domainModelTypes, modelIds) = e.GetTypeAndModelData();
            await _bus.PublishAsync(new RefreshEvent(domainModelTypes, modelIds));
        }
    }
}