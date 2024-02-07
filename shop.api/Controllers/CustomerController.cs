using Microsoft.AspNetCore.Mvc;
using shop.eventsourcing;
using shop.shared;

namespace shop.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController(
    IEventBus _eventBus)
    : ControllerBase
{
    [HttpPost]
    public  Task Post([FromBody] CustomerPost Customer) => //validate and then
        _eventBus.PublishAsync(new NewCustomerEvent(Guid.NewGuid(), Customer.Name, Customer.Address));

    [HttpPatch("{id}")]
    public Task Patch(Guid id, [FromBody] CustomerPatch Customer) => //validate and then
        _eventBus.PublishAsync(new UpdateCustomerAddressEvent(id, Customer.Address));
}

public record CustomerPatch(string Address);
public record CustomerPost(string Name, string Address);