using Microsoft.AspNetCore.Mvc;

namespace shop.api;

[Route("api/[controller]")]
[ApiController]
public class FruitController : ControllerBase
{
    [HttpGet]
    public IEnumerable<Fruit?> Get()
    {
        return EventStore.FruitEvents()
            .GroupBy(f => f.ModelId)
            .Select(g => g.ToModel())
            .Where(f => f is not null);
    }

    [HttpGet("historical")]
    public IEnumerable<IEnumerable<Fruit?>> GetHistoricalAll()
    {
        return EventStore.FruitEvents()
            .GroupBy(f => f.ModelId)
            .Select(g => g.ToModelHistorical())
            .Where(f => f is not null);
    }

    [HttpGet("{id}")]
    public Fruit? Get(Guid id)
    {
        return EventStore.FruitEvents()
            .Where(f => f.ModelId == id)
            .ToModel();
    }

    [HttpGet("{id}/historical")]
    public IEnumerable<Fruit?> GetHistorical(Guid id)
    {
        return EventStore.FruitEvents().Where(e => e.ModelId == id)
            .ToModelHistorical();
    }

    [HttpPost]
    public void Post([FromBody] Fruit value)
    {
    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody] Fruit value)
    {
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
