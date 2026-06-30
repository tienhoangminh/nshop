namespace nShop.Catalog.QueryHandlers
{
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Result<GetCategoryResponse>>
    {
        private readonly IEventRepository eventStore;

        public GetCategoryQueryHandler(IEventRepository eventStore)
        {
            this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        public async Task<Result<GetCategoryResponse>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var category = await eventStore.FindAsync<Category>(request.CategoryId, cancellationToken: cancellationToken);
            if (category == null)
            {
                return Result<GetCategoryResponse>.Failure(new ResultError("CategoryNotFound", $"Category with ID {request.CategoryId} not found."));
            }

            return Result<GetCategoryResponse>.Success(new GetCategoryResponse() { Category = category });
        }
    }
    
    public class GetCategoryQuery : IRequest<Result<GetCategoryResponse>>
    {
        public Guid CategoryId { get; set; }
    }
}
