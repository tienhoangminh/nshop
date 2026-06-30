using MediatR;

namespace nShop.Infrastructure.KafkaPullService;

public class EventNotification(object @event) : INotification
{
    public object Event { get; } = @event ?? throw new ArgumentNullException(nameof(@event));
}

public class EventNotification<TEvent>(TEvent @event) : INotification
{
    public TEvent Event { get; } = @event ?? throw new ArgumentNullException(nameof(@event));
}