
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using shop.api.Controllers;
using shop.eventsourcing;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
                .AddMvcOptions(o => o.Conventions.Add(new GenericControllerAttribute()))
                .ConfigureApplicationPartManager(c =>
                {
                    c.FeatureProviders.Add(new GenericControllerFeatureProvider());
                });
builder.Services.AddSharedDependencies(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers().WithOpenApi();

app.Run();


public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var domainModelTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(DomainModel).IsAssignableFrom(type) && !type.IsAbstract);

        foreach (Type type in domainModelTypes)
        {
            Type controllerType = typeof(QueryController<>).MakeGenericType(type);
            feature.Controllers.Add(controllerType.GetTypeInfo());
        }
    }
}