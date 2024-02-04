using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using shop.eventsourcing;
using shop.shared;

namespace Microsoft.Extensions.DependencyInjection;

public static class SharedServiceCollectionExtensions
{
    public static IServiceCollection AddSharedDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ShopDb");
        services.AddDbContext<ShopDbContext>(opt =>
        {
            opt.UseSqlServer(connectionString, ctxOptions =>
            {
                ctxOptions.MigrationsAssembly("shop.api");
            });
        });

        services
            .AddTransient<IEventStore, LocalDbEventStore>()
            .AddSingleton<IEventBus, InMemoryEventBus>();

        services.RegisterDomainDependencies();

        return services;
    }

    public static IServiceCollection RegisterDomainDependencies(this IServiceCollection services)
    {
        var domainModelTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(DomainModel).IsAssignableFrom(type) && !type.IsAbstract);

        foreach (var domainModelType in domainModelTypes)
        {
            var addMethod = typeof(SharedServiceCollectionExtensions)
                .GetMethod(nameof(AddDomainTypeDependencies));

            var genericMethod = addMethod.MakeGenericMethod(domainModelType);
            genericMethod.Invoke(null, new object[] { services });
        }

        return services;
    }

    public static IServiceCollection AddDomainTypeDependencies<T>(this IServiceCollection services) where T : DomainModel
    {
        return services
            .AddTransient<IDomainService<T>, DomainService<T>>()
            .AddTransient<IQueryService<T>, QueryService<T>>()
            .AddTransient<ICacheRefresher<T>, CacheRefresher<T>>()
            .AddHostedService<CacheRefresher<T>>();
    }

}
