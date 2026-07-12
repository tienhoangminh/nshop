using nShop.Catalog.Api.Handlers.Commands;
using nShop.Catalog.Client.Abstractions.Dtos;

namespace nShop.Catalog.Api;

internal partial class CatalogApi
{
    public override async Task<GrpcAddVariationDimensionsResponse> AddVariationDimensions(
        GrpcAddVariationDimensionsRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.ProductId, out Guid productId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ProductId"));

        var command = new AddVariationDimensionsCommand()
        {
            ProductId = productId,
            Dimensions = request.Dimensions.Select(d => new VariationDimensionDto()
            {
                Name = d.Name,
                DisplayName = d.DisplayName,
                DisplayStyle = d.DisplayStyle == GrpcVariationDimensionDisplayStyles.Color
                    ? VariationDisplayStyles.Color
                    : VariationDisplayStyles.Text,
                Values = [.. d.Values]
            }).ToArray()

        };

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcAddVariationDimensionsResponse
            {
            };
        }

        throw new RpcException(new Status(StatusCode.Internal,
            result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
    }
}
