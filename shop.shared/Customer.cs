using shop.eventsourcing;

namespace shop.shared;
public record Customer(Guid Id, string Name, string Address, Dictionary<Guid, int> FruitHoldings) : DomainModel(Id);

public record NewCustomerEvent(Guid CustomerId, string Name, string Address, DateTimeOffset AppliesAt) : EventBase, IEvent<Customer>
{
    public Guid ModelId { get; set; } = CustomerId;
    public DateTimeOffset AppliesAt { get; set; } = AppliesAt;
    public  Customer? Apply(Customer? existing)
    {
        return new Customer(CustomerId, Name, Address, []);
    }
}

public record UpdateCustomerAddressEvent(Guid CustomerId, string? Address, DateTimeOffset AppliesAt) : EventBase, IEvent<Customer>
{
    public Guid ModelId { get; set; } = CustomerId;
    public DateTimeOffset AppliesAt { get; set; } = AppliesAt;
    public Customer? Apply(Customer? existing)
    {
        return existing is null ? null : existing with
        {
            Address = Address is null ? existing.Address : Address
        };
    }
}