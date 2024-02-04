using shop.eventsourcing;

namespace shop.shared;
public record Customer(Guid Id, string Name, string Address, int Holdings) : DomainModel(Id);

public record NewCustomerEvent(Guid CustomerId, string Name, string Address) : EventBase, IEvent<Customer>
{
    public Guid ModelId { get; set; } = CustomerId;
    public  Customer Apply(Customer? existing)
    {
        return new Customer(CustomerId, Name, Address, 0);
    }
}

public record UpdateCustomerAddressEvent(Guid CustomerId, string? Address) : EventBase, IEvent<Customer>
{
    public Guid ModelId { get; set; } = CustomerId;
    public Customer Apply(Customer? existing)
    {
        return existing! with
        {
            Address = Address is null ? existing.Address : Address
        };
    }
}