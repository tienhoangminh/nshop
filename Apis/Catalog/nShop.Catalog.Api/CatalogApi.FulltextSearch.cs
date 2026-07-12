using nShop.Catalog.Elasticsearch;
using static nShop.Catalog.Api.GrpcFulltextSearchResponse.Types;

namespace nShop.Catalog.Api;

internal partial class CatalogApi
{
    public override async Task<GrpcFulltextSearchResponse> FulltextSearch(GrpcFulltextSearchRequest request,
        ServerCallContext context)
    {
        var offset = request.Paging.Offset;
        if (offset < 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Offset must be greater than or equal to 0"));
        var limit = request.Paging.Limit;
        if (limit < 1)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Limit must be greater than 0"));

        // todo: use filters

        var searchResponse = await elasticsearchClient.SearchAsync<ProductIndexDocument>(s => s
            .From((int?)offset)
            .Size((int?)limit)
            .Query(q => q
                .Term(t => t.Field(d => d.Name).CaseInsensitive(true).Value(request.Query))
            )
        );

        if (searchResponse.IsValidResponse)
        {
            var docs = searchResponse.Documents;
            var response = new GrpcFulltextSearchResponse
            {
            };

            response.Products.AddRange(docs.Select(d => new GrpcFulltextSearchProduct
            {
                ProductId = d.Id.ToString(),
                Name = d.Name,
            }));

            return response;
        }

        return new GrpcFulltextSearchResponse();
    }
}
