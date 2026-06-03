namespace nShop.Catalog.SyncService.MongoDb;

public class SyncHandlerException :Exception
{
    public SyncHandlerException() { }
    public SyncHandlerException(string message) : base(message)
    {
    }
}