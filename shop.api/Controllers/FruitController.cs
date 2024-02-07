using Microsoft.AspNetCore.Mvc;
using shop.eventsourcing;
using shop.shared;

namespace shop.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FruitController(
    IEventBus _eventBus)
    : ControllerBase
{
    [HttpPost]
    public async Task<Fruit> Post([FromBody] FruitPost fruit)
    {
        //validate and then
        var guid = Guid.NewGuid();
        await _eventBus.PublishAsync(new CreateFruitEvent(guid, fruit.Name, fruit.Color));
        return new Fruit(guid, fruit.Name, fruit.Color);
    }

    [HttpPatch("{id}")]
    public Task Patch(Guid id, [FromBody] FruitPatch fruit) => //validate and then
        _eventBus.PublishAsync(new UpdateFruitEvent(id, fruit.Color));

    [HttpDelete("{id}")]
    public Task Delete(Guid id) => //validate and then
        _eventBus.PublishAsync(new DeleteFruitEvent(id));

    [HttpPost("{id}/sell")]
    public Task Test(Guid id, [FromBody] SellFruitPost sellFruit) => //validate and then
        _eventBus.PublishAsync<IEvent<Fruit>>(new SellFruitEvent(id, sellFruit.CustomerId, sellFruit.Amount));
}

public record FruitPatch(string Color);
public record FruitPost(string Name, string Color);
public record SellFruitPost(Guid CustomerId, int Amount);