namespace nShop.Catalog.SyncService.MongoDb;

public class MongoDbReadModelSyncFactory : IReadModelSyncFactory
{
    private readonly IMongoClient mongoDbClient;
    private readonly ILoggerFactory loggerFactory;
    private readonly IMapper mapper;
    private readonly string database;
    
    
    
    public MongoDbReadModelSyncFactory(IMongoClient mongoDbClient, ILoggerFactory loggerFactory, Action<MongoDbReadModelSyncFactoryConfig>? configAction = null)
    {
        var config = new MongoDbReadModelSyncFactoryConfig();
        configAction?.Invoke(config);

        this.mongoDbClient = mongoDbClient;
        this.database = config.Database;
        this.loggerFactory = loggerFactory;

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<CatalogEvent2DtoProfile>());
        mapper = mapperConfig.CreateMapper();
    }
    
    public IReadModelSyncEventHandler GetSyncEventHandler()
    {
        return new MongoDbReadModelSyncEventHandler(mongoDbClient, database, mapper, loggerFactory.CreateLogger<MongoDbReadModelSyncEventHandler>());
    }

    public IReadModelSyncDataReader<T> GetReader<T>() where T : class, IDto
    {
        return new MongoDbReadModelSyncDataReader<T>(mongoDbClient, database, loggerFactory.CreateLogger<MongoDbReadModelSyncDataReader<T>>());
    }
}

public class MongoDbReadModelSyncFactoryConfig
{
    public string Database { get; set; } = "Catalog";
}