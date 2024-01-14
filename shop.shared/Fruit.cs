using shop.eventsourcing;

namespace shop.shared;

public record Fruit(Guid Id, string Name, string Color) : DomainModel(Id);
