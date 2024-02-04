﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using shop.eventsourcing;
using System.Text.Json;

namespace shop.shared;
public interface ICacheRefresher<T> where T : DomainModel
{
    Task RefreshCache(CancellationToken stoppingToken);
}

public class CacheRefresher<T>(
    IEventBus _bus,
    IServiceScopeFactory _scopeFactory)
    : BackgroundService, ICacheRefresher<T> where T : DomainModel
{
    public async Task RefreshCache(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var _shopDbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
        var _domainService = scope.ServiceProvider.GetRequiredService<IDomainService<T>>();
        var historical = _domainService.GetHistorical();

        await _shopDbContext.ReadModels
            .Where(rm => rm.DomainModelType == typeof(T).FullName!)
            .ExecuteDeleteAsync(cancellationToken: stoppingToken);

        await _shopDbContext.ReadModels.AddRangeAsync(
            historical.SelectMany(l => 
            l.Select(m =>
                new ReadModel(m.UpdatedAt, typeof(T).FullName!, m.Id, JsonSerializer.Serialize(m)))),
            stoppingToken);

        await _shopDbContext.SaveChangesAsync(stoppingToken);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var events = _bus.Subscribe<T>();
        await foreach (var e in events)
        {
            using var scope = _scopeFactory.CreateScope();
            var _shopDbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
            var _domainService = scope.ServiceProvider.GetRequiredService<IDomainService<T>>();

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            foreach (var id in e.Ids)
            {
                try
                {
                    var historical = _domainService.GetHistorical(id);

                    await _shopDbContext.ReadModels
                        .Where(rm => rm.Id == id)
                        .ExecuteDeleteAsync(cancellationToken: stoppingToken);

                    var idHolder = historical.FirstOrDefault(m => m?.Id != null);

                    await _shopDbContext.ReadModels.AddRangeAsync(
                        historical.Select(m => new ReadModel(m.UpdatedAt, typeof(T).FullName!, m.Id, JsonSerializer.Serialize(m)))
                    , stoppingToken);

                    await _shopDbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception)
                {
                    //probably doesn't exist
                }
            }
        }
    }
}