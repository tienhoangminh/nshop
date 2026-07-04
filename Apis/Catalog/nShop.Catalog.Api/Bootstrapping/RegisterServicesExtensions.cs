using nShop.Infrastructure.EventBus.Kafka;
using nShop.Shared;

namespace nShop.Catalog.Api.Bootstrapping;

public static class RegisterServicesExtensions
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterServices();
        builder.RegisterEventBus();
        builder.RegisterSyncFactory();
    }
    
    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddAutoMapper(typeof(Program).Assembly);
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(Product).Assembly);
        });
        services.AddValidatorsFromAssemblyContaining<Product>();
    }

    private static void RegisterEventBus(this WebApplicationBuilder builder)
    {
        builder.AddKafkaProducer<string, string>(ServiceConnectionNames.EventBus);
        builder.Services.AddKafkaEventBus(ServiceConnectionNames.CatalogEventBusTopic);
    }

    private static void RegisterSyncFactory(this WebApplicationBuilder builder)
    {
    }
}