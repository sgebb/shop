namespace shop.eventsourcing;

public interface IEventStore
{
    IEnumerable<IEvent<T>> Events<T>() where T : DomainModel;
    IEnumerable<IEvent<T>> EventsFor<T>(Guid modelId) where T : DomainModel;
    void AddEvent<T>(IEvent<T> e) where T : DomainModel;
}
