using System.Text;

namespace shop.api;

public static class Extensions
{
    public static T? ToModel<T>(this IEnumerable<Event<T>> events) where T : DomainModel =>
        events.ToModelHistorical().Last();

    public static IEnumerable<T?> ToModelHistorical<T>(this IEnumerable<Event<T>> events) where T : DomainModel
    {
        T? current = null;

        foreach(var e in events)
        {
            current = e.On(current);
            yield return current;
        }
    }

    public static Guid ToGuid(this string src)
    {
        byte[] stringbytes = Encoding.UTF8.GetBytes(src);
        byte[] hashedBytes = System.Security.Cryptography
        .SHA1.HashData(stringbytes);
        Array.Resize(ref hashedBytes, 16);
        return new Guid(hashedBytes);
    }
}
