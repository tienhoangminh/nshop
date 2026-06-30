using Microsoft.Extensions.DependencyInjection;

namespace nShop.Infrastructure.KafkaPullService;

public static class KafkaPullServiceExtensions
{
    public static void UseKafkaPullServiceWorker(this IServiceCollection services, Action<KafkaPullServiceWorkerOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHostedService<KafkaPullServiceWorker>();
    }
}