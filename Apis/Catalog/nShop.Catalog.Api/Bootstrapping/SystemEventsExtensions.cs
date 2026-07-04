using nShop.Catalog.IntegrationEvents;
using nShop.Core.IntegrationEvent;

namespace nShop.Catalog.Api.Bootstrapping;

public static class SystemEventsExtensions
{
    public async static Task PublishSystemStartedEventAsync(this IApplicationBuilder app, CancellationToken cancellationToken = default)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        await eventBus.PublishAsync(new SystemStartedIntegrationEvent(), cancellationToken);
    }
}