namespace nShop.Catalog.Aggregates;

public class Product : IAggregate, ITenancyEntity
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public double Price { get; set; }
    public string[] Tags { get; set; } = [];
    public string Slug { get; set; } = string.Empty;
    public ProductStates State { get; set; } = ProductStates.Draft;
    public string[] Images { get; set; } = [];
    public Guid[] Groups { get; set; } = [];
    public Guid TenantId { get; set; }
    public List<Variant> Variants { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ulong Version { get; set; }
    public IEnumerable<IDomainEvent> PendingEvents => _changes;

    private readonly List<ProductEvent> _changes = [];
    public Product() {
    }
    
    public static Product Create(Guid id, Guid tenantId, string name, string description, Guid categoryId, double price, string[] tags, string slug, string[] images, Guid[] groups, DateTime? timeStamp = null)
    {
        var product = new Product
        {
            ProductId = id,
            TenantId = tenantId,
            Name = name,
            Description = description,
            CategoryId = categoryId,
            Price = price,
            Tags = tags,
            Slug = slug,
            Images = images,
            Groups = groups,
            CreatedAt = timeStamp ?? DateTime.UtcNow,
            UpdatedAt = timeStamp ?? DateTime.UtcNow,

            Version = 0,
        };

        var @event = new ProductCreatedEvent() {
            ProductId = id,
            TenantId = tenantId,
            Name = name,
            Description = description,
            CategoryId = categoryId,
            Price = price,
            Tags = tags,
            Slug = slug,
            Images = images,
            Groups = groups,
            Timestamp = product.CreatedAt
        };

        product.Apply(@event);
        product._changes.Add(@event);

        return product;
    }
    
    //Why version does not increase when update ? Does it have any specific purpose? Then when version will increase?
    public void Update(string name, string description, Guid categoryId, string[] tags, string slug, string[] images, Guid[] groups)
    {
        var @event = new ProductUpdatedEvent()
        {
            ProductId = ProductId,
            TenantId = TenantId,
            Name = name,
            Description = description,
            CategoryId = categoryId,
            Tags = tags,
            Slug = slug,
            Images = images,
            Groups = groups,
            Timestamp = DateTime.UtcNow
        };

        Apply(@event);
        _changes.Add(@event);
    }
    
    public void Publish()
    {
        var @event = new ProductPublishedEvent()
        {
            ProductId = ProductId,
            Timestamp = DateTime.UtcNow
        };
        Apply(@event);
        _changes.Add(@event);
    }

    public void Unpublish()
    {
        var @event = new ProductUnpublishedEvent()
        {
            ProductId = ProductId,
            Timestamp = DateTime.UtcNow
        };
        Apply(@event);
        _changes.Add(@event);
    }
    public void Project(object @event)
    {
        switch (@event)
        {
            case ProductUpdatedEvent e: // ProductUpdatedEvent inherits from ProductCreatedEvent so it must be before ProductCreatedEvent 
                Apply(e);
                break;
            case ProductCreatedEvent e:
                Apply(e);
                break;
            case ProductPublishedEvent e:
                Apply(e);
                break;
            case ProductUnpublishedEvent e:
                Apply(e);
                break;
            // Handle other events
        }
    }
    
    #region Apply Events

    private void Apply(ProductCreatedEvent evt)
    {
        ProductId = evt.ProductId;
        TenantId = evt.TenantId;
        Name = evt.Name;
        Description = evt.Description;
        CategoryId = evt.CategoryId;
        Price = evt.Price;
        Tags = evt.Tags;
        Slug = evt.Slug;
        Images = evt.Images;
        Groups = evt.Groups;
    }

    private void Apply(ProductUpdatedEvent evt)
    {
        Name = evt.Name;
        Description = evt.Description;
        CategoryId = evt.CategoryId;
        Tags = evt.Tags;
        Slug = evt.Slug;
        Images = evt.Images;
        Groups = evt.Groups;
        TenantId = evt.TenantId;
    }

    private void Apply(ProductPublishedEvent e)
    {
        if (State == ProductStates.Published)
        {
            throw new InvalidOperationException("Product is already published");
        }

        State = ProductStates.Published;
    }
    private void Apply(ProductUnpublishedEvent e)
    {
        if (State != ProductStates.Published)
        {
            throw new InvalidOperationException("Product is not published");
        }

        State = ProductStates.Draft;
    }

    #endregion
    
    public void ResetEvents()
    {
        throw new NotImplementedException();
    }
}