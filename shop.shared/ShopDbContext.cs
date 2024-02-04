using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace shop.shared;

public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<IEventEntity> IEvents { get; set; }
    public DbSet<ReadModel> ReadModels { get; set; }
}

public record ReadModel(DateTimeOffset At, string DomainModelType, Guid Id, string Content)
{
    [Key]
    public Guid ReadModelId { get; set; }
};

public record IEventEntity(
    List<Guid> ModelId,
    DateTimeOffset CreatedAt,
    List<string> DomainModelType,
    string EventType,
    string Content)
{
    [Key]
    public Guid EventId { get; set; }
}