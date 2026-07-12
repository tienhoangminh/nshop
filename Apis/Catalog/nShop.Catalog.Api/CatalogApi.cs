using Elastic.Clients.Elasticsearch;
using nShop.Catalog.Client.Abstractions.Dtos;
using nShop.Catalog.SyncService.Abstractions;

namespace nShop.Catalog.Api;

internal partial class CatalogApi : CatalogService.CatalogServiceBase
{
    private readonly ILogger<CatalogApi> logger;
    private readonly IReadModelSyncDataReader<ProductDto> productDtoReader;
    private readonly IReadModelSyncDataReader<CategoryDto> categoryDtoReader;
    private readonly ElasticsearchClient elasticsearchClient;
    private readonly IMediator mediator;
    private readonly IMapper mapper;

    public CatalogApi(
        IReadModelSyncDataReader<ProductDto> productDtoReader,
        IReadModelSyncDataReader<CategoryDto> categoryDtoReader,
        ElasticsearchClient elasticsearchClient,
        IMediator mediator, IMapper mapper, ILogger<CatalogApi> logger)
    {
        this.productDtoReader = productDtoReader;
        this.categoryDtoReader = categoryDtoReader;
        this.elasticsearchClient = elasticsearchClient;
        this.mediator = mediator;
        this.mapper = mapper;
        this.logger = logger;
    }
}