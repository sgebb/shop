using Microsoft.AspNetCore.Mvc;
using shop.eventsourcing;
using shop.shared;

namespace shop.api;

[Route("api/[controller]")]
[ApiController]
public class FruitController(IEventStore _eventStore)
    : ControllerBase
{
    [HttpGet]
    public IEnumerable<Fruit?> Get(DateTimeOffset? at = null) => _eventStore.Events<Fruit>()
            .GroupBy(f => f.ModelId)
            .Select(g => g.ToModel(at))
            .Where(f => f is not null);

    [HttpGet("historical")]
    public IEnumerable<IEnumerable<Fruit?>> GetHistoricalAll() => _eventStore.Events<Fruit>()
            .GroupBy(f => f.ModelId)
            .Select(g => g.ToModelHistorical())
            .Where(f => f is not null);

    [HttpGet("{id}")]
    public Fruit? Get(Guid id, DateTimeOffset? at = null) => _eventStore
            .Events<Fruit>(id)
            .ToModel(at);

    [HttpGet("{id}/historical")]
    public IEnumerable<Fruit?> GetHistorical(Guid id) => _eventStore
            .Events<Fruit>(id)
            .ToModelHistorical();

    [HttpPost]
    public void Post([FromBody] Fruit fruit) => 
        _eventStore.AddEvent(new CreateFruitEvent(Guid.NewGuid(), fruit.Name, fruit.Color));

    [HttpPatch("{id}")]
    public void Patch(Guid id, [FromBody] Fruit fruit) => 
        _eventStore.AddEvent(new UpdateFruitEvent(id, fruit.Name, fruit.Color));

    [HttpDelete("{id}")]
    public void Delete(Guid id) => 
        _eventStore.AddEvent(new DeleteFruitEvent(id));
}
