namespace nShop.Catalog.SyncService.MongoDb;

internal class MongoDbReadModelSyncDataReader<T> : IReadModelSyncDataReader<T> where T : class, IDto
{
    protected readonly IMongoClient mongoDbClient;
    protected readonly ILogger<MongoDbReadModelSyncDataReader<T>> logger;
    protected readonly IMongoCollection<T> entityCollection;
    
    public MongoDbReadModelSyncDataReader(IMongoClient mongoDbClient, string database, ILogger<MongoDbReadModelSyncDataReader<T>> logger)
    {
        this.mongoDbClient = mongoDbClient;
        this.logger = logger;
        
        entityCollection = mongoDbClient.GetDatabase(database).GetCollection<T>(typeof(T).Name);
    }
    
    public async Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var ps = await entityCollection.FindAsync(p => p.Id == id, cancellationToken: cancellationToken);

            var p = ps.FirstOrDefault(cancellationToken: cancellationToken) as T;
            if (p == null)
            {
                logger.LogWarning("Item not found by ID: {id}", id);
            }
            else {
                logger.LogDebug("Item found by ID: {id}", id);
            }

            return p;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while finding entity by ID: {id}", id);
            throw;
        }
    }
}