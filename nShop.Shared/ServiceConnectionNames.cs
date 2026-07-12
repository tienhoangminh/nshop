namespace nShop.Shared;

public static class ServiceConnectionNames
{
    public const string CatalogEventBusTopic = "catalog";
    public const string CatalogProjectionBus = "catalog-projection";
    public const string EventBus = "event-bus";
    public const string MongoDb = "mongo";
    public const string Elasticsearch = "elasticsearch";
    public const string Redis = "redis";
}