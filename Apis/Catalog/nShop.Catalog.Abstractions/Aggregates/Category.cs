namespace nShop.Catalog.Aggregates;

public class Category : IAggregate, ITenancyEntity
{
    public Guid Id { get; private set; }
    public Guid CategoryId => Id;
    public Guid TenantId { get; set; }
    public Guid ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public CategoryStates State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ulong Version { get; set; }
    public IEnumerable<IDomainEvent> PendingEvents => _changes;

    private readonly List<CategoryEvent> _changes = [];
    
    //Why version equal 1 at initial? Does it have any specific purpose?
    public Category()
    {
        Version++;
    }
    public Category(IEnumerable<CategoryEvent> events)
    {
        foreach (var e in events)
        {
            Project(e);
            Version++;
        }
    }
    
    public static Category Create(Guid id, Guid parentId, string name, string slug)
    {
        var category = new Category();
        var @event = new CategoryCreatedEvent { CategoryId = id, ParentId = parentId, Name = name, Slug = slug, Timestamp = DateTime.UtcNow };
        category.Apply(@event);
        category._changes.Add(@event);
        return category;
    }

    public Category Update(string name, string slug, Guid parentId)
    {
        var @event = new CategoryUpdatedEvent { CategoryId = CategoryId, Name = name, Slug = slug, ParentId = parentId, Timestamp = DateTime.UtcNow };
        Apply(@event);
        _changes.Add(@event);
        return this;
    }
    
    public void Publish(DateTime? timeStamp = null)
    {
        var @event = new CategoryPublishedEvent()
        {
            CategoryId = CategoryId,
            Timestamp = timeStamp ?? DateTime.UtcNow
        };
        Apply(@event);
        _changes.Add(@event);
    }
    
    public void Unpublish(DateTime? timeStamp = null)
    {
        var @event = new CategoryUnpublishedEvent()
        {
            CategoryId = CategoryId,
            Timestamp = timeStamp ?? DateTime.UtcNow
        };
        Apply(@event);
        _changes.Add(@event);
    }
    
    private void Apply(CategoryCreatedEvent e)
    {
        Id = e.CategoryId;
        Name = e.Name;
        CreatedAt = e.Timestamp;
        UpdatedAt = e.Timestamp;
        Slug = e.Slug;
        ParentId = e.ParentId;
    }

    private void Apply(CategoryUpdatedEvent e)
    {
        Name = e.Name;
        ParentId = e.ParentId;
        Slug = e.Slug;

        UpdatedAt = e.Timestamp;
    }
    
    private void Apply(CategoryPublishedEvent e)
    {
        State = CategoryStates.Published;

        UpdatedAt = e.Timestamp;
    }
    
    private void Apply(CategoryUnpublishedEvent e)
    {
        State = CategoryStates.Unpublished;

        UpdatedAt = e.Timestamp;
    }
    
    public void Project(object @event)
    {
        switch (@event)
        {
            case CategoryCreatedEvent e:
                Apply(e);
                break;
            case CategoryUpdatedEvent e:
                Apply(e);
                break;
            case CategoryPublishedEvent e:
                Apply(e);
                break;
            case CategoryUnpublishedEvent e:
                Apply(e);
                break;
        }
    }
    
    public void ResetEvents()
    {
        _changes.Clear();
    }
}

public enum CategoryStates {
    Unpublished = 0,
    Published = 1,
}