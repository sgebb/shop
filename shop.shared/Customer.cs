using shop.eventsourcing;

namespace shop.shared;
public record Customer(Guid Id, string Name, string Address) : DomainModel(Id);

public record NewCustomerEvent(Guid CustomerId, string Name, string Address, DateTimeOffset AppliesAt) : Event<Customer>(CustomerId, AppliesAt)
{
    public override Customer? Apply(Customer? existing)
    {
        return new Customer(CustomerId, Name, Address);
    }
}

public record UpdateCustomerAddressEvent(Guid CustomerId, string? Address, DateTimeOffset AppliesAt) : Event<Customer>(CustomerId, AppliesAt)
{
    public override Customer? Apply(Customer? existing)
    {
        return existing is null ? null : existing with
        {
            Address = Address is null ? existing.Address : Address
        };
    }
}