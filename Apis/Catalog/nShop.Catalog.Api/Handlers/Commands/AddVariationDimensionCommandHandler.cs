using nShop.Catalog.Client.Abstractions.Dtos;

namespace nShop.Catalog.Api.Handlers.Commands;

public class AddVariationDimensionCommandHandler : IRequestHandler<AddVariationDimensionsCommand, Result>
{
    private readonly IValidator<AddVariationDimensionsCommand> validator;
    private readonly IEventRepository eventStore;
    private readonly IMediator mediator;
    private readonly ILogger<AddVariationDimensionCommandHandler> logger;

    public AddVariationDimensionCommandHandler(IValidator<AddVariationDimensionsCommand> validator,
        IEventRepository eventStore, IMediator mediator, ILogger<AddVariationDimensionCommandHandler> logger)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this.logger = logger;
    }

    public async Task<Result> Handle(AddVariationDimensionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure(
                    validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)));
            }

            var product = await eventStore.FindAsync<Product>(request.ProductId, cancellationToken: cancellationToken);
            if (product == null)
            {
                return Result.Failure(new ValidationError("ProductId", "Product not found"));
            }

            foreach (var dimension in request.Dimensions)
            {
                product.AddVariationDimension(dimension.Name, dimension.DisplayName, dimension.DisplayStyle,
                    dimension.Values);
            }

            await eventStore.UpdateAsync(product.Id, product, cancellationToken: cancellationToken);

            foreach (var @event in product.PendingEvents)
            {
                await mediator.Publish(new DomainEventWrapper(@event), cancellationToken);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding variation dimensions");
            return Result.Failure(new ResultError("AddVariationDimensionsError", ex.Message));
        }
    }
}

public class AddVariationDimensionsCommand : IRequest<Result>
{
    public Guid ProductId { get; set; }
    public required VariationDimensionDto[] Dimensions { get; set; }
}

public class AddVariationDimensionRequestsValidator : AbstractValidator<AddVariationDimensionsCommand>
{
    public AddVariationDimensionRequestsValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required.");
        RuleFor(x => x.Dimensions).NotEmpty().WithMessage("Dimensions is required.");
    }
}