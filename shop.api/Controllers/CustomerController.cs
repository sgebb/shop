using Microsoft.AspNetCore.Mvc;
using shop.shared;

namespace shop.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController(IDomainService<Customer> _CustomerService)
    : ControllerBase
{
    [HttpGet]
    public IEnumerable<Customer?> Get(DateTimeOffset? at = null) => 
        _CustomerService.Get(at);

    [HttpGet("historical")]
    public IEnumerable<IEnumerable<Customer?>> GetHistorical() =>
        _CustomerService.GetHistorical();

    [HttpGet("{id}")]
    public Customer? Get(Guid id, DateTimeOffset? at = null) => 
        _CustomerService.Get(id, at);

    [HttpGet("{id}/historical")]
    public IEnumerable<Customer?> GetHistorical(Guid id) => 
        _CustomerService.GetHistorical(id);

    [HttpPost]
    public void Post([FromBody] CustomerPost Customer) => //validate and then
        _CustomerService.AddEvent(new NewCustomerEvent(Guid.NewGuid(), Customer.Name, Customer.Address, Customer.AppliesAt));

    [HttpPatch("{id}")]
    public void Patch(Guid id, [FromBody] CustomerPatch Customer) => //validate and then
        _CustomerService.AddEvent(new UpdateCustomerAddressEvent(id, Customer.Address, Customer.AppliesAt));
}

public record CustomerPatch(string Address, DateTimeOffset AppliesAt);
public record CustomerPost(string Name, string Address, DateTimeOffset AppliesAt);