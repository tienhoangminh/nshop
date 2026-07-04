using nShop.Core.IntegrationEvent;
using nShop.Shared;

namespace nShop.Catalog.IntegrationEvents;

public class SystemStartedIntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; } = GuidHelpers.NewGuidVersion7();
    public DateTime StartTime { get; } = DateTime.UtcNow;
    public string SystemName { get; private set; } = "Catalog";

    public SystemStartedIntegrationEvent()
    {
    }
    public SystemStartedIntegrationEvent(string systemName)
    {
        SystemName = systemName;
    }
}