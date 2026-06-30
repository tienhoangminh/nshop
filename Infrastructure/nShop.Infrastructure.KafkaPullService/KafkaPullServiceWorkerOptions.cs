namespace nShop.Infrastructure.KafkaPullService;

public class KafkaPullServiceWorkerOptions
{
    public required string ServiceName { get; set; }
    public required string BootstrapServers { get; set; }
    public required string GroupId { get; set; }
    public required string Topic { get; set; }
    public Func<string, Type?>? LoadEventType { get; set; }
}