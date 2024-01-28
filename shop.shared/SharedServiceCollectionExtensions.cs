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
            .AddTransient<IDomainService<Fruit>, DomainService<Fruit>>()
            .AddTransient<IDomainService<Customer>, DomainService<Customer>>()
            ;

        return services;
    }
}
