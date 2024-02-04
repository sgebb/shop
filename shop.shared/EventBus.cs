using shop.eventsourcing;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace shop.shared;

public record RefreshEvent(IEnumerable<Type> Types, IEnumerable<Guid> Ids);

public interface IEventBus
{
    IAsyncEnumerable<RefreshEvent> Subscribe<T>() where T : DomainModel;
    void Publish(RefreshEvent e);
}

public class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentBag<ChannelWriter<RefreshEvent>> writers = [];

    public void Publish(RefreshEvent e)
    {
        foreach (var writer in writers)
        {
            writer.TryWrite(e);
        }
    }

    public async IAsyncEnumerable<RefreshEvent> Subscribe<T>() where T : DomainModel
    {
        var channel = Channel.CreateUnbounded<RefreshEvent>();
        writers.Add(channel.Writer);
        while (await channel.Reader.WaitToReadAsync())
        {
            while (channel.Reader.TryRead(out RefreshEvent e))
            {
                if (e.Types.Any(m => m == typeof(T)))
                {
                    yield return e;
                }
            }
        }
    }
}