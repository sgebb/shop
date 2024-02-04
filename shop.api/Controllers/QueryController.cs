using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using shop.eventsourcing;
using shop.shared;

namespace shop.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[GenericController]
public class QueryController<T>(
    IQueryService<T> _queryService,
    ICacheRefresher<T> _cacheRefresher) 
    : ControllerBase where T : DomainModel
{
    [HttpGet]
    public IEnumerable<T?> Get(DateTimeOffset? at = null) =>
        _queryService.Get(at);

    [HttpGet("historical")]
    public IEnumerable<IEnumerable<T?>> GetHistorical() =>
        _queryService.GetHistorical();

    [HttpGet("{id}")]
    public T? Get(Guid id, DateTimeOffset? at = null) =>
        _queryService.Get(id, at);

    [HttpGet("{id}/historical")]
    public IEnumerable<T?> GetHistorical(Guid id) =>
        _queryService.GetHistorical(id);

    [HttpPost("refreshCache")]
    public Task RefreshCache(CancellationToken stoppingToken) =>
        _cacheRefresher.RefreshCache(stoppingToken);
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class GenericControllerAttribute : Attribute, IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var entityType = controller.ControllerType.GenericTypeArguments.FirstOrDefault();

        if (entityType != null && typeof(DomainModel).IsAssignableFrom(entityType))
        {
            controller.ControllerName = entityType.Name;
        }
    }
}