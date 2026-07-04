using nShop.Catalog.Api.Bootstrapping;
using nShop.Infrastructure.EventStore.EventStoreDB;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.
builder.RegisterServices();

builder.Services.AddTransient<IEventRepository>(services => new EventStoreDBRepository(
    builder.Configuration.GetConnectionString("eventstore") ?? throw new InvalidOperationException("EventStoreDB connection string is missing.")
));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CatalogApi>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

//Change from Run to RunAsync so we can inject own custom logic right after application start
var task = app.RunAsync();

await app.PublishSystemStartedEventAsync();

await task;

