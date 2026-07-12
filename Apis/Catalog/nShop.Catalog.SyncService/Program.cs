var builder = Host.CreateApplicationBuilder(args);
builder.AddAppDefaults();

builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.UseKafkaPullServiceWorker(options =>
{
    options.ServiceName = "nShop.Catalog.SyncService";
    options.BootstrapServers = builder.Configuration.GetConnectionString(ServiceConnectionNames.EventBus) ?? throw new ArgumentNullException(ServiceConnectionNames.EventBus);
    options.GroupId = builder.Configuration["Kafka:GroupId"] ?? throw new ArgumentNullException("Kafka:GroupId");
    options.Topic = builder.Configuration["Kafka:Topic"] ?? throw new ArgumentNullException("Kafka:Topic");
    options.LoadEventType = (t) =>
    {
        return typeof(SystemStartedIntegrationEvent).Assembly.GetType(t);
    };
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.NotificationPublisher = new TaskWhenAllPublisher();
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.AddRedisDistributedCache(ServiceConnectionNames.Redis);

builder.RegisterSyncFactory(ServiceConnectionNames.MongoDb);
builder.RegisterElasticSearch(ServiceConnectionNames.Elasticsearch);

var host = builder.Build();
host.Run();