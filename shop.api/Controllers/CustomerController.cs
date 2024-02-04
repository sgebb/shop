﻿using Microsoft.AspNetCore.Mvc;
using shop.shared;

namespace shop.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController(
    IDomainService<Customer> _CustomerService)
    : ControllerBase
{
    [HttpPost]
    public void Post([FromBody] CustomerPost Customer) => //validate and then
        _CustomerService.AddEvent(new NewCustomerEvent(Guid.NewGuid(), Customer.Name, Customer.Address));

    [HttpPatch("{id}")]
    public void Patch(Guid id, [FromBody] CustomerPatch Customer) => //validate and then
        _CustomerService.AddEvent(new UpdateCustomerAddressEvent(id, Customer.Address));
}

public record CustomerPatch(string Address);
public record CustomerPost(string Name, string Address);