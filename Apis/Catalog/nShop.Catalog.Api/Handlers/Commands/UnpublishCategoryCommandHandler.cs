namespace nShop.Catalog.Api.Handlers.Commands;

public class UnpublishCategoryCommandHandler : IRequestHandler<UnpublishCategoryCommand, Result>
{
    private readonly IValidator<UnpublishCategoryCommand> validator;
    private readonly IEventRepository eventStore;
    private readonly IMediator mediator;

    public UnpublishCategoryCommandHandler(IValidator<UnpublishCategoryCommand> validator, IEventRepository eventStore,
        IMediator mediator)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Result> Handle(UnpublishCategoryCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.Errors.Select(e =>
                new Core.SeedWork.Results.ValidationError(e.PropertyName, e.ErrorMessage)));
        }

        var category =
            await eventStore.FindAsync<Category>(command.CategoryId, null, cancellationToken: cancellationToken);
        if (category == null)
        {
            return Result.NotFound();
        }

        category.Unpublish();

        await eventStore.UpdateAsync(category.CategoryId, category, cancellationToken: cancellationToken);

        foreach (var @event in category.PendingEvents)
        {
            await mediator.Publish(new DomainEventWrapper(@event), cancellationToken);
        }

        //category.ResetEvents();

        return Result.Success();
    }
}

public class UnpublishCategoryCommand : IRequest<Result>
{
    public Guid CategoryId { get; set; }
}

public class UnpublishCategoryCommandValidator : AbstractValidator<UnpublishCategoryCommand>
{
    public UnpublishCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
