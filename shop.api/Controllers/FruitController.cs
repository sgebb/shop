using Microsoft.AspNetCore.Mvc;
using shop.shared;

namespace shop.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FruitController(IDomainService<Fruit> _fruitService)
    : ControllerBase
{
    [HttpGet]
    public IEnumerable<Fruit?> Get(DateTimeOffset? at = null) => 
        _fruitService.Get(at);

    [HttpGet("historical")]
    public IEnumerable<IEnumerable<Fruit?>> GetHistorical() =>
        _fruitService.GetHistorical();

    [HttpGet("{id}")]
    public Fruit? Get(Guid id, DateTimeOffset? at = null) => 
        _fruitService.Get(id, at);

    [HttpGet("{id}/historical")]
    public IEnumerable<Fruit?> GetHistorical(Guid id) => 
        _fruitService.GetHistorical(id);

    [HttpPost]
    public void Post([FromBody] FruitPost fruit) => //validate and then
        _fruitService.AddEvent(new CreateFruitEvent(Guid.NewGuid(), fruit.Name, fruit.Color));

    [HttpPatch("{id}")]
    public void Patch(Guid id, [FromBody] FruitPatch fruit) => //validate and then
        _fruitService.AddEvent(new UpdateFruitEvent(id, fruit.Color));

    [HttpDelete("{id}")]
    public void Delete(Guid id) => //validate and then
        _fruitService.AddEvent(new DeleteFruitEvent(id));

    [HttpPost("{id}/sell")]
    public void Test(Guid id, [FromBody] SellFruitPost sellFruit) => //validate and then
        _fruitService.AddEvent(new SellFruitEvent(id, sellFruit.CustomerId, sellFruit.Amount));
}

public record FruitPatch(string Color);
public record FruitPost(string Name, string Color);
public record SellFruitPost(Guid CustomerId, int Amount);