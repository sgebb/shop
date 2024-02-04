using System.Collections.Concurrent;
using System.Threading.Channels;

namespace shop.shared;

public record RefreshEvent(IEnumerable<Type> Types, IEnumerable<Guid> Ids);

public interface IEventBus
{
    IAsyncEnumerable<T> Subscribe<T>();
    void Publish<T>(T e);
}

public class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentBag<ChannelWriter<object>> writers = [];

    public void Publish<T>(T e)
    {
        foreach (var writer in writers)
        {
            writer.TryWrite(e);
        }
    }

    public async IAsyncEnumerable<T> Subscribe<T>()
    {
        var channel = Channel.CreateUnbounded<object>();
        writers.Add(channel.Writer);
        while (await channel.Reader.WaitToReadAsync())
        {
            while (channel.Reader.TryRead(out object e))
            {
                if (e is T wantedEvent)
                {
                    yield return wantedEvent;
                }
            }
        }
    }
}