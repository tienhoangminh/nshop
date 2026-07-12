
namespace nShop.Catalog.Api.Handlers.Commands;

public class PublishProductCommandHandler(
    IValidator<PublishProductCommand> validator,
    IEventRepository eventStore,
    IMediator mediator,
    IMapper mapper) : IRequestHandler<PublishProductCommand, Result>
{
    private readonly IValidator<PublishProductCommand> validator =
        validator ?? throw new ArgumentNullException(nameof(validator));

    private readonly IEventRepository eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
    private readonly IMediator mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<Result> Handle(PublishProductCommand command, CancellationToken cancellationToken)
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

        product.Publish();

        await eventStore.UpdateAsync(product.ProductId, product, cancellationToken: cancellationToken);

        foreach (var @event in product.PendingEvents)
        {
            await mediator.Publish(new DomainEventWrapper(@event), cancellationToken);
        }

        // var productPublishedEvent = new ProductPublishedIntegrationEvent()
        // {
        //     ProductId = command.ProductId,
        // };
        // await eventBus.PublishAsync(productPublishedEvent, cancellationToken);

        return Result.Success();
    }
}

public class PublishProductCommand : IRequest<Result>
{
    public Guid ProductId { get; set; }
}

public class PublishProductCommandValidator : AbstractValidator<PublishProductCommand>
{
    public PublishProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}