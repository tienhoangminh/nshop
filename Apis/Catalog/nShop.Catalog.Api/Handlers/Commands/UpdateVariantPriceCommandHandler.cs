namespace nShop.Catalog.Api.Handlers.Commands
{
    public class UpdateVariantPriceCommandHandler : IRequestHandler<UpdateVariantPriceRequest, Result>
    {
        private readonly IValidator<UpdateVariantPriceRequest> validator;
        private readonly IEventRepository eventStore;
        private readonly IMediator mediator;
        private readonly ILogger<UpdateVariantCommandHandler> logger;

        public UpdateVariantPriceCommandHandler(IValidator<UpdateVariantPriceRequest> validator, IEventRepository eventStore, IMediator mediator, ILogger<UpdateVariantCommandHandler> logger)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger;
        }

        public async Task<Result> Handle(UpdateVariantPriceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return Result.Failure(validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)));
                }

                var product = await eventStore.FindAsync<Product>(request.ProductId, cancellationToken: cancellationToken);
                if (product == null)
                {
                    return Result.Failure(new ValidationError("ProductId", "Product not found"));
                }

                product.UpdateVariantPrice(
                    request.VariantId, 
                    request.Price,
                    request.DiscountPrice);

                await eventStore.UpdateAsync(product.Id, product, cancellationToken: cancellationToken);

                foreach (var @event in product.PendingEvents)
                {
                    await mediator.Publish(new DomainEventWrapper(@event), cancellationToken);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updateing variant price");
                return Result.Failure(new ResultError("UpdateVariantPriceError", ex.Message));
            }
        }
    }

    public class UpdateVariantPriceRequest : IRequest<Result>
    {
        public Guid VariantId { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
    }

    public class UpdateVariantPriceValidator : AbstractValidator<UpdateVariantPriceRequest>
    {
        public UpdateVariantPriceValidator()
        {
            RuleFor(x => x.VariantId).NotEmpty().WithMessage("VariantId is required.");
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required.");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Price is required.");
            RuleFor(x => x.DiscountPrice).NotEmpty().WithMessage("DiscountPrice is required.");
        }
    }

}
