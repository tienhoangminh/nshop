using System.Text.Json.Serialization;

namespace nShop.Core.SeedWork;

public interface IAggregate : IProjection
{
    Guid Id { get; }
    ulong Version { get; set; }
    [JsonIgnore]
    IEnumerable<IDomainEvent> PendingEvents { get; }
    void ResetEvents();
}