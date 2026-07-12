using Microsoft.Extensions.DependencyInjection;
using nShop.Catalog.Api;

namespace nShop.Catalog.Client;

public static class CatalogClientRegistrationExtensions
{
    public static IServiceCollection RegisterCatalogClient(this IServiceCollection services, string url) {

        services.AddGrpcClient<CatalogService.CatalogServiceClient>(o =>
        {
            o.Address = new Uri(url);
        }).AddServiceDiscovery();

        return services;
    }
}