namespace nShop.Catalog.Api.Handlers.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IValidator<UpdateProductCommand> validator;
    private readonly IEventRepository eventStore;
    private readonly IMediator mediator;
    private readonly IMapper mapper;

    public UpdateProductCommandHandler(IValidator<UpdateProductCommand> validator, IEventRepository eventStore,
        IMediator mediator, IMapper mapper)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.Errors.Select(e =>
                new Core.SeedWork.Results.ValidationError(e.PropertyName, e.ErrorMessage)));
        }

        var product =
            await eventStore.FindAsync<Product>(request.ProductId, null, cancellationToken: cancellationToken);
        if (product == null)
        {
            return Result.NotFound();
        }

        product.Update(request.Name, request.Description, request.CategoryId, [.. request.Tags], request.Slug,
            [.. request.Images], [.. request.Groups]);

        await eventStore.UpdateAsync(product.CategoryId, product, cancellationToken: cancellationToken);

        foreach (var @event in product.PendingEvents)
        {
            await mediator.Publish(new DomainEventWrapper(@event), cancellationToken);
        }

        //var productUpdatedEvent = mapper.Map<ProductUpdatedIntegrationEvent>(product);
        //await eventBus.PublishAsync(productUpdatedEvent, cancellationToken);

        return Result.Success();
    }
}

public class UpdateProductCommand : IRequest<Result>
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string[] Tags { get; set; } = [];
    public string Slug { get; set; } = string.Empty;
    public string[] Images { get; set; } = [];
    public Guid[] Groups { get; set; } = [];
}
