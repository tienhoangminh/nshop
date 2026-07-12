namespace nShop.Catalog.Aggregates;

public class Category : IAggregate, ITenancyEntity
{
    public Guid ProductId { get; private set; }
    public Guid TenantId { get; set; }
    public Guid ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
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
        var @event = new CategoryUpdatedEvent { CategoryId = ProductId, Name = name, Slug = slug, ParentId = parentId, Timestamp = DateTime.UtcNow };
        Apply(@event);
        _changes.Add(@event);
        return this;
    }
    
    private void Apply(CategoryCreatedEvent e)
    {
        ProductId = e.CategoryId;
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
        }
    }
    
    public void ResetEvents()
    {
        _changes.Clear();
    }
}