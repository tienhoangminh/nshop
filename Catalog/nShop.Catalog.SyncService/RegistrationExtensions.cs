namespace nShop.Catalog.SyncService;

public static class RegistrationExtensions
{
    public static IHostApplicationBuilder RegisterSyncFactory(this IHostApplicationBuilder builder,
        string connectionName)
    {
        var handlerType = builder.Configuration["SyncHandlerType"];
        if ("MongoDb".Equals(handlerType, StringComparison.OrdinalIgnoreCase))
        {
            builder.AddMongoDBClient(connectionName);
            builder.Services.AddTransient<IReadModelSyncFactory, MongoDbReadModelSyncFactory>();
        }
        else
        {
            throw new NotSupportedException($"Handler type {handlerType} is not supported.");
        }

        return builder;
    }

    public static IHostApplicationBuilder RegisterElasticSearch(this IHostApplicationBuilder builder,
        string connectionName)
    {
        //Concern:Not sure why we need IsExternalServiceMode here?
        //Why we have ElasticSearch outside of Aspire?
        if (builder.Configuration["IsExternalServiceMode"] == "false")
        {
            builder.AddElasticsearchClient(connectionName: connectionName,
                configureClientSettings: (settings) =>
                {
                    settings.DefaultMappingFor<ProductIndexDocument>(m => m.IndexName(nameof(ProductIndexDocument).ToLower()));
                }
            );
        }
        else
        {
            var settings = new ElasticsearchClientSettings(new Uri(builder.Configuration.GetConnectionString(connectionName) ?? throw new Exception($"Connection string '{connectionName}' not found")));
            settings.DefaultMappingFor<ProductIndexDocument>(m => m.IndexName(nameof(ProductIndexDocument).ToLower()));

            builder.Services.AddTransient(services => new ElasticsearchClient(
                settings
            ));
        }

        return builder;
    }
}