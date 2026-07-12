namespace nShop.Catalog.Api.Handlers.Commands;

public class UpdateVariantCommandHandler : IRequestHandler<UpdateVariantCommand, Result<UpdateVariantResponse>>
{
    private readonly IValidator<UpdateVariantCommand> validator;
    private readonly IEventRepository eventStore;
    private readonly IMediator mediator;
    private readonly ILogger<UpdateVariantCommandHandler> logger;

    public UpdateVariantCommandHandler(IValidator<UpdateVariantCommand> validator, IEventRepository eventStore,
        IMediator mediator, ILogger<UpdateVariantCommandHandler> logger)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this.logger = logger;
    }

    public async Task<Result<UpdateVariantResponse>> Handle(UpdateVariantCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<UpdateVariantResponse>.Failure(
                    validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)));
            }

            var product = await eventStore.FindAsync<Product>(request.ProductId, cancellationToken: cancellationToken);
            if (product == null)
            {
                return Result<UpdateVariantResponse>.Failure(new ValidationError("ProductId", "Product not found"));
            }

            product.UpdateVariant(
                request.VariantId,
                request.Name,
                request.Sku);

            await eventStore.UpdateAsync(product.Id, product, cancellationToken: cancellationToken);

            foreach (var @event in product.PendingEvents)
            {
                await mediator.Publish(new DomainEventWrapper(@event), cancellationToken);
            }

            return Result<UpdateVariantResponse>.Success(new UpdateVariantResponse() { VariantId = request.VariantId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating variant");
            return Result<UpdateVariantResponse>.Failure(new ResultError("UpdateVariantError", ex.Message));
        }
    }
}

public class UpdateVariantCommand : IRequest<Result<UpdateVariantResponse>>
{
    public Guid VariantId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
}

public class UpdateVariantResponse
{
    public Guid VariantId { get; set; }
}

public class UpdateVariantValidator : AbstractValidator<UpdateVariantCommand>
{
    public UpdateVariantValidator()
    {
        RuleFor(x => x.VariantId).NotEmpty().WithMessage("VariantId is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }
}