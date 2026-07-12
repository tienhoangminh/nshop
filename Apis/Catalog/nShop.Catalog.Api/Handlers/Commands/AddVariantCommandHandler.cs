using nShop.Shared;

namespace nShop.Catalog.Api.Handlers.Commands;

public class AddVariantCommandHandler : IRequestHandler<AddVariantCommand, Result<AddVariantResponse>>
{
    private readonly IValidator<AddVariantCommand> validator;
    private readonly IEventRepository eventStore;
    private readonly IMediator mediator;
    private readonly ILogger<AddVariantCommandHandler> logger;

    public AddVariantCommandHandler(IValidator<AddVariantCommand> validator, IEventRepository eventStore,
        IMediator mediator, ILogger<AddVariantCommandHandler> logger)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this.logger = logger;
    }

    public async Task<Result<AddVariantResponse>> Handle(AddVariantCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<AddVariantResponse>.Failure(
                    validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)));
            }

            var product = await eventStore.FindAsync<Product>(request.ProductId, cancellationToken: cancellationToken);
            if (product == null)
            {
                return Result<AddVariantResponse>.Failure(new ValidationError("ProductId", "Product not found"));
            }

            var variantId = GuidHelpers.NewGuidVersion7();
            product.AddVariant(
                variantId,
                request.Name,
                request.Sku,
                request.Price,
                request.DiscountPrice,
                request.DimensionValues.Select(v => new Aggregates.VariantDimensionValue()
                {
                    Name = v.Name,
                    Value = v.Value
                }),
                false);

            await eventStore.UpdateAsync(product.Id, product, cancellationToken: cancellationToken);

            foreach (var @event in product.PendingEvents)
            {
                await mediator.Publish(new DomainEventWrapper(@event), cancellationToken);
            }

            return Result<AddVariantResponse>.Success(new AddVariantResponse() { VariantId = variantId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding variation dimensions");
            return Result<AddVariantResponse>.Failure(new ResultError("AddVariationDimensionsError", ex.Message));
        }
    }
}

public class AddVariantCommand : IRequest<Result<AddVariantResponse>>
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public List<AddVariantDimensionValue> DimensionValues { get; set; } = [];
}

public class AddVariantDimensionValue
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class AddVariantResponse
{
    public Guid VariantId { get; set; }
}

public class AddVariantValidator : AbstractValidator<AddVariantCommand>
{
    public AddVariantValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }
}
