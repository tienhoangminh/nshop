using MediatR;
using nShop.Shared;

namespace nShop.Core.IntegrationEvent;

public abstract class IntegrationEvent : IIntegrationEvent, INotification
{
    public Guid EventId { get;}
    public DateTime Timestamp { get; set; }

    public IntegrationEvent()
    {
        EventId = GuidHelpers.NewGuidVersion7();
        Timestamp = DateTime.UtcNow;
    }

    public IntegrationEvent(Guid eventId, DateTime timestamp)
    {
        EventId = eventId;
        Timestamp = timestamp;
    }
}