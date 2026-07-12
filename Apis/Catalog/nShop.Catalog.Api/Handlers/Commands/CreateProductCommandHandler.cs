using nShop.Shared;

namespace nShop.Catalog.Api.Handlers.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IValidator<CreateProductCommand> validator;
    private readonly IEventRepository eventStore;
    private readonly IMediator mediator;

    public CreateProductCommandHandler(IValidator<CreateProductCommand> validator, IEventRepository eventStore, IMediator mediator)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<CreateProductResponse>.Failure(validationResult.Errors.Select(e => new Core.SeedWork.Results.ResultError("Validation", e.PropertyName)));
        }

        var id = GuidHelpers.NewGuidVersion7();
        var product = Product.Create(id, command.TenantId, command.Name, command.Description, command.CategoryId, command.Price, command.Tags.ToArray(), command.Slug, command.Images.ToArray(), command.Groups.ToArray());

        await eventStore.AddAsync(id, product, cancellationToken);

        foreach (var @event in product.PendingEvents)
        {
            await mediator.Publish(new DomainEventWrapper(@event), cancellationToken);
        }

        product.ResetEvents();

        return Result<CreateProductResponse>.Success(new CreateProductResponse { ProductId = id });
    }
}

public class CreateProductCommand : ProductCommandBase, IRequest<Result<CreateProductResponse>>, ITenancyEntity
{
    public double Price { get; set; }

    public CreateProductCommand()
    {
    }

    public CreateProductCommand(string productName, string description, Guid categoryId, double price, IEnumerable<string> tags, string slug, IEnumerable<string> images, IEnumerable<Guid> groups, Guid tenantId)
    {
        Name = productName;
        Description = description;
        CategoryId = categoryId;
        Price = price;
        Tags = tags;
        Slug = slug;
        Images = images;
        Groups = groups;
        TenantId = tenantId;
    }
}

public class CreateProductResponse
{
    public Guid ProductId { get; set; }
}
