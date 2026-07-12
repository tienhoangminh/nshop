using nShop.Catalog.Api.Handlers.Commands;

namespace nShop.Catalog.Api;

internal partial class CatalogApi
{
    public override async Task<GrpcCreateCategoryResponse> CreateCategory(GrpcCreateCategoryRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.TenantId, out Guid tenantId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid TenantId"));
        if (Guid.TryParse(request.ParentId, out Guid parentId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ParentId"));
        try
        {
            var command = mapper.Map<CreateCategoryCommand>(request);

            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                if (result.Value == null)
                    throw new RpcException(new Status(StatusCode.Internal, "Invalid state: result.Value == null"));

                return new GrpcCreateCategoryResponse
                {
                    CategoryId = result.Value.Id.ToString()
                };
            }
            else
            {
                throw new RpcException(new Status(StatusCode.Internal, result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
            }
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<GrpcUpdateCategoryResponse> UpdateCategory(GrpcUpdateCategoryRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.CategoryId, out Guid categoryId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid CategoryId"));
        if (Guid.TryParse(request.ParentId, out Guid parentId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ParentId"));

        var command = new UpdateCategoryCommand(
            categoryId,
            parentId,
            request.Name,
            request.Slug
            );

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcUpdateCategoryResponse
            {
            };
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Internal, result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
        }
    }

    public override async Task<GrpcFindCategoryByIdResponse> FindCategoryById(GrpcFindCategoryByIdRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.CategoryId, out Guid categoryId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid CategoryId"));

        var findResult = await categoryDtoReader.FindByIdAsync(categoryId);

        if (findResult == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Category not found"));

        return new GrpcFindCategoryByIdResponse
        {
            Category = mapper.Map<GrpcCategory>(findResult)
        };

    }

    public async override Task<GrpcPublishCategoryResponse> PublishCategory(GrpcPublishCategoryRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.CategoryId, out Guid categoryId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid CategoryId"));

        var command = new PublishCategoryCommand()
        {
            CategoryId = categoryId
        };

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcPublishCategoryResponse
            {
            };
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Internal, result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
        }
    }
    public async override Task<GrpcUnpublishCategoryResponse> UnpublishCategory(GrpcUnpublishCategoryRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.CategoryId, out Guid categoryId) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid CategoryId"));

        var command = new UnpublishCategoryCommand()
        {
            CategoryId = categoryId
        };

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GrpcUnpublishCategoryResponse
            {
            };
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Internal, result.Errors.Any() ? string.Join(',', result.Errors.Select(e => e.ErrorMessage)) : "Unknown error"));
        }
    }
}