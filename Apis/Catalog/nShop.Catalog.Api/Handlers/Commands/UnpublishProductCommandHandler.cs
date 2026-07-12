namespace nShop.Catalog.Api.Handlers.Commands;

public class UnpublishProductCommandHandler(
    IValidator<UnpublishProductCommand> validator,
    IEventRepository eventStore,
    IMediator mediator) : IRequestHandler<UnpublishProductCommand, Result>
{
    private readonly IValidator<UnpublishProductCommand> validator =
        validator ?? throw new ArgumentNullException(nameof(validator));

    private readonly IEventRepository eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
    private readonly IMediator mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


    public async Task<Result> Handle(UnpublishProductCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.Errors.Select(e =>
                new Core.SeedWork.Results.ValidationError(e.PropertyName, e.ErrorMessage)));
        }

        var product =
            await eventStore.FindAsync<Product>(command.ProductId, null, cancellationToken: cancellationToken);
        if (product == null)
        {
            return Result.NotFound();
        }

        product.Unpublish();

        await eventStore.UpdateAsync(product.ProductId, product, cancellationToken: cancellationToken);

        foreach (var @event in product.PendingEvents)
        {
            await mediator.Publish(new DomainEventWrapper(@event), cancellationToken);
        }

        // var productUnpublishedEvent = new ProductUnpublishedIntegrationEvent()
        // {
        //     ProductId = command.ProductId,
        // };
        // await eventBus.PublishAsync(productUnpublishedEvent, cancellationToken);

        return Result.Success();
    }
}

public class UnpublishProductCommand : IRequest<Result>
{
    public Guid ProductId { get; set; }
}

public class UnpublishProductCommandValidator : AbstractValidator<UnpublishProductCommand>
{
    public UnpublishProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
