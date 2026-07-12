using nShop.Catalog.Api.Handlers.Commands;

namespace nShop.Catalog.Api;

internal partial class CatalogApi
{
    public override async Task<GrpcCreateProductResponse> CreateProduct(GrpcCreateProductRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.CategoryId, out Guid categoryId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid CategoryId"));

        if (Guid.TryParse(request.TenantId, out Guid tenantId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid TenantId"));

        var command = mapper.Map<CreateProductCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            if (result.Value == null)
                throw new RpcException(new Status(StatusCode.Internal, "Invalid state: result.Value == null"));

            return new GrpcCreateProductResponse() { ProductId = result.Value.ProductId.ToString() };
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Internal, result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
        }
    }

    public override async Task<GrpcUpdateProductResponse> UpdateProduct(GrpcUpdateProductRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.ProductId, out Guid productId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ProductId"));

        if (Guid.TryParse(request.CategoryId, out Guid categoryId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid CategoryId"));

        var command = mapper.Map<UpdateProductCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcUpdateProductResponse();
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Internal, result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
        }
    }

    public override async Task<GrpcFindProductByIdResponse> FindProductById(GrpcFindProductByIdRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.ProductId, out Guid productId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ProductId"));

        var findResult = await productDtoReader.FindByIdAsync(productId);

        if (findResult == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

        try
        {
            var product = mapper.Map<GrpcProduct>(findResult);

            return new GrpcFindProductByIdResponse
            {
                Product = product
            };
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error mapping product");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<GrpcPublishProductResponse> PublishProduct(GrpcPublishProductRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.ProductId, out Guid productId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ProductId"));

        var command = mapper.Map<PublishProductCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcPublishProductResponse();
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Internal, result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
        }
    }

    public override async Task<GrpcUnpublishProductResponse> UnpublishProduct(GrpcUnpublishProductRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.ProductId, out Guid productId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ProductId"));

        var command = mapper.Map<UnpublishProductCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcUnpublishProductResponse();
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Internal, result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
        }
    }
}