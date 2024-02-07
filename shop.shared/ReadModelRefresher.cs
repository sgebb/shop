using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using shop.eventsourcing;
using System.Text.Json;

namespace shop.shared;
public interface IReadModelRefresher<T> where T : DomainModel
{
    Task RefreshCache(CancellationToken stoppingToken);
}

public class ReadModelRefresher<T>(
    IEventBus _bus,
    IServiceScopeFactory _scopeFactory)
    : BackgroundService, IReadModelRefresher<T> where T : DomainModel
{
    public async Task RefreshCache(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var _shopDbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
        var _domainService = scope.ServiceProvider.GetRequiredService<IDomainService<T>>();
        var historical = _domainService.GetHistorical();

        _shopDbContext.ReadModels.RemoveRange(
            _shopDbContext.ReadModels.Where(rm => rm.DomainModelType == typeof(T).FullName!).AsEnumerable()
            );

        await _shopDbContext.ReadModels.AddRangeAsync(
            historical.SelectMany(l =>
            l.Select(m =>
                new ReadModel(m.UpdatedAt, typeof(T).FullName!, m.Id, JsonSerializer.Serialize(m)))),
            stoppingToken);

        await _shopDbContext.SaveChangesAsync(stoppingToken);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var events = _bus.Subscribe<RefreshEvent>();
        await foreach (var e in events)
        {
            using var scope = _scopeFactory.CreateScope();
            var _shopDbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
            var _domainService = scope.ServiceProvider.GetRequiredService<IDomainService<T>>();

            foreach (var id in e.Ids)
            {
                try
                {
                    var historical = _domainService.GetHistorical(id);

                    _shopDbContext.ReadModels.RemoveRange(
                        _shopDbContext.ReadModels.Where(rm => rm.DomainModelType == typeof(T).FullName! && rm.Id == id).AsEnumerable()
                        );

                    var idHolder = historical.FirstOrDefault(m => m?.Id != null);

                    await _shopDbContext.ReadModels.AddRangeAsync(
                        historical.Select(m => new ReadModel(m.UpdatedAt, typeof(T).FullName!, m.Id, JsonSerializer.Serialize(m)))
                    , stoppingToken);

                    await _shopDbContext.SaveChangesAsync(stoppingToken);
                    await _bus.PublishAsync(new ReadModelsUpdated(typeof(T), id));
                }
                catch (Exception)
                {
                    // combination of T and id doesn't exist, not unexpected
                }
            }
        }
    }
}