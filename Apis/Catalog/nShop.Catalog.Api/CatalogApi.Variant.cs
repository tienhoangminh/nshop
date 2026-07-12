using nShop.Catalog.Api.Handlers.Commands;

namespace nShop.Catalog.Api;

internal partial class CatalogApi
{
    public override async Task<GrpcAddVariantResponse> AddVariant(GrpcAddVariantRequest request,
        ServerCallContext context)
    {
        if (Guid.TryParse(request.ProductId, out Guid productId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ProductId"));

        var command = mapper.Map<AddVariantCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcAddVariantResponse
            {
            };
        }

        throw new RpcException(new Status(StatusCode.Internal,
            result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
    }

    public override async Task<GrpcUpdateVariantResponse> UpdateVariant(GrpcUpdateVariantRequest request,
        ServerCallContext context)
    {
        if (Guid.TryParse(request.VariantId, out Guid variantId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid VariantId"));

        var command = mapper.Map<UpdateVariantCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcUpdateVariantResponse
            {
            };
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Internal,
                result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
        }
    }

    public override async Task<GrpcUpdateVariantPriceResponse> UpdateVariantPrice(GrpcUpdateVariantPriceRequest request,
        ServerCallContext context)
    {
        if (Guid.TryParse(request.VariantId, out Guid variantId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid VariantId"));

        var command = mapper.Map<UpdateVariantPriceRequest>(request);

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcUpdateVariantPriceResponse();
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Internal,
                result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
        }
    }
}
