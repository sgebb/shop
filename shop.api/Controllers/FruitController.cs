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
    public Fruit Post([FromBody] FruitPost fruit)
    {
        //validate and then
        var guid = Guid.NewGuid();
        _eventBus.Publish(new CreateFruitEvent(guid, fruit.Name, fruit.Color));
        return new Fruit(guid, fruit.Name, fruit.Color);
    }

    [HttpPatch("{id}")]
    public void Patch(Guid id, [FromBody] FruitPatch fruit) => //validate and then
        _eventBus.Publish(new UpdateFruitEvent(id, fruit.Color));

    [HttpDelete("{id}")]
    public void Delete(Guid id) => //validate and then
        _eventBus.Publish(new DeleteFruitEvent(id));

    [HttpPost("{id}/sell")]
    public void Test(Guid id, [FromBody] SellFruitPost sellFruit) => //validate and then
        _eventBus.Publish<IEvent<Fruit>>(new SellFruitEvent(id, sellFruit.CustomerId, sellFruit.Amount));
}

public record FruitPatch(string Color);
public record FruitPost(string Name, string Color);
public record SellFruitPost(Guid CustomerId, int Amount);