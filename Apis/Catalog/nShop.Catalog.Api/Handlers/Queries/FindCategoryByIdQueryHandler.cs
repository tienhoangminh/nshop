using nShop.Core.SeedWork.Results;

namespace nShop.Catalog.Api.Handlers.Queries;

public class FindCategoryByIdQueryHandler : IRequestHandler<FindCategoryByIdRequest, Result<FindCategoryByIdResponse>>
{
    private readonly IValidator<FindCategoryByIdRequest> validator;
    private readonly IEventRepository eventStore;
    private readonly IMapper mapper;

    public FindCategoryByIdQueryHandler(IValidator<FindCategoryByIdRequest> validator, IEventRepository eventStore,
        IMapper mapper)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<FindCategoryByIdResponse>> Handle(FindCategoryByIdRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<FindCategoryByIdResponse>.Failure(
                validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)));
        }

        var category = await eventStore.FindAsync<Category>(request.CategoryId, cancellationToken: cancellationToken);
        if (category == null)
        {
            return Result<FindCategoryByIdResponse>.Failure(new ResultError("CategoryNotFound",
                $"Category with ID {request.CategoryId} not found."));
        }

        return Result<FindCategoryByIdResponse>.Success(mapper.Map<FindCategoryByIdResponse>(category));
    }
}

public class FindCategoryByIdRequest : IRequest<Result<FindCategoryByIdResponse>>
{
    public Guid CategoryId { get; set; }
}

public class FindCategoryByIdResponse
{
    public Guid CategoryId { get; set; }
    public Guid TenantId { get; set; }
    public Guid ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
