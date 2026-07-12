namespace nShop.Catalog.Aggregates;

public class Product : IAggregate, ITenancyEntity
{
    public Guid Id { get; private set; }
    public Guid ProductId => Id;
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
    public List<VariationDimension> VariationDimensions { get; set; } = [];

    public Product()
    {
    }

    public static Product Create(Guid id, Guid tenantId, string name, string description, Guid categoryId, double price,
        string[] tags, string slug, string[] images, Guid[] groups, DateTime? timeStamp = null)
    {
        var product = new Product
        {
            Id = id,
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

        var @event = new ProductCreatedEvent()
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
            Timestamp = product.CreatedAt
        };

        product.Apply(@event);
        product._changes.Add(@event);

        return product;
    }

    //Why version does not increase when update ? Does it have any specific purpose? Then when version will increase?
    public void Update(string name, string description, Guid categoryId, string[] tags, string slug, string[] images,
        Guid[] groups)
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

    public void AddVariant(Guid variantId, string name, string sku, double price, double discountPrice,
        IEnumerable<VariantDimensionValue> values, bool isDefault, DateTime? timeStamp = null)
    {
        var @event = new VariantCreatedEvent()
        {
            ProductId = Id,
            VariantId = variantId,
            Name = name,
            Sku = sku,
            Price = price,
            DiscountPrice = discountPrice,
            IsDefault = isDefault,
            VariantDimensionValues = new(values),
            Timestamp = timeStamp ?? DateTime.UtcNow
        };

        Apply(@event);
        _changes.Add(@event);
    }

    public void AddVariationDimension(string name, string displayName, VariationDisplayStyles displayStyle,
        string[] values, DateTime? timeStamp = null)
    {
        var @event = new VariationDimensionAddedEvent()
        {
            ProductId = Id,
            Name = name,
            DisplayName = displayName,
            DisplayStyle = displayStyle,
            Values = values,
            Timestamp = timeStamp ?? DateTime.UtcNow
        };
        Apply(@event);
        _changes.Add(@event);
    }

    public void UpdateVariant(Guid variantId, string name, string sku, DateTime? timeStamp = null)
    {
        if (!Variants.Any(v => v.Id == variantId))
        {
            throw new ArgumentException("Variant Id not found");
        }

        var @event = new VariantUpdatedEvent()
        {
            ProductId = Id,
            VariantId = variantId,
            Name = name,
            Sku = sku,
            Timestamp = timeStamp ?? DateTime.UtcNow
        };

        Apply(@event);
        _changes.Add(@event);
    }

    public void UpdateVariantPrice(Guid variantId, double price, double discountPrice, DateTime? timeStamp = null)
    {
        if (!Variants.Any(v => v.Id == variantId))
        {
            throw new ArgumentException("Variant Id not found");
        }

        var @event = new VariantPriceChangedEvent()
        {
            ProductId = Id,
            VariantId = variantId,
            Price = price,
            DiscountPrice = discountPrice,
            Timestamp = timeStamp ?? DateTime.UtcNow
        };

        Apply(@event);
        _changes.Add(@event);
    }

    public void Project(object @event)
    {
        switch (@event)
        {
            case ProductUpdatedEvent e
                : // ProductUpdatedEvent inherits from ProductCreatedEvent so it must be before ProductCreatedEvent 
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
            case ProductDeletedEvent e:
                Apply(e);
                break;
            case VariationDimensionAddedEvent e:
                Apply(e);
                break;
            case VariationDimensionUpdatedEvent e:
                Apply(e);
                break;
            case VariationDimensionValuesChangedEvent e:
                Apply(e);
                break;
            case VariantCreatedEvent e:
                Apply(e);
                break;
            case VariantPublishedEvent e:
                Apply(e);
                break;
            case VariantUnpublishedEvent e:
                Apply(e);
                break;

            // Handle other events
        }

        if (@event is ICatalogEvent evt)
        {
            UpdatedAt = evt.Timestamp;
        }
    }

    #region Apply Events

    private void Apply(ProductCreatedEvent evt)
    {
        Id = evt.ProductId;
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

    private void Apply(ProductDeletedEvent evt)
    {
        if (State == ProductStates.Deleted)
        {
            throw new InvalidOperationException("Product is already deleted");
        }

        State = ProductStates.Deleted;
    }

    private void Apply(VariantCreatedEvent evt)
    {
        if (evt.IsDefault)
        {
            if (evt.VariantDimensionValues.Count != 0)
            {
                throw new InvalidOperationException("Default variant must not have dimension values");
            }

            if (Variants.Any(x => x.IsDefault))
            {
                throw new InvalidOperationException("Default variant already exists");
            }
        }
        else
        {
            if (VariationDimensions.Count != evt.VariantDimensionValues.Count)
            {
                throw new InvalidOperationException("Too many or not enough values provided to dimensions");
            }

            foreach (var dimension in VariationDimensions)
            {
                if (!evt.VariantDimensionValues.Any(x => x.Name == dimension.Name))
                {
                    throw new InvalidOperationException($"Dimension value not provided for {dimension.Name}");
                }
            }
        }

        var variant = new Variant
        {
            Id = evt.VariantId,
            ProductId = evt.ProductId,
            Name = evt.Name,
            Sku = evt.Sku,
            Price = evt.Price,
            DiscountPrice = evt.DiscountPrice,
            IsDefault = evt.IsDefault,
            State = VariantStates.Unpublished,
            VariantDimensionValues = evt.VariantDimensionValues,
        };

        Variants.Add(variant);
    }

    private void Apply(VariationDimensionAddedEvent evt)
    {
        var dimension = new VariationDimension
        {
            Name = evt.Name,
            DisplayName = evt.DisplayName,
            DisplayStyle = evt.DisplayStyle,
            Values = evt.Values
        };

        VariationDimensions.Add(dimension);
    }

    private void Apply(VariantUpdatedEvent evt)
    {
        var variant = Variants.FirstOrDefault(x => x.Id == evt.VariantId) ??
                      throw new InvalidOperationException("Variant not found");
        variant.Name = evt.Name;
        variant.Sku = evt.Sku;
    }

    private void Apply(VariantPriceChangedEvent evt)
    {
        var variant = Variants.FirstOrDefault(x => x.Id == evt.VariantId) ??
                      throw new InvalidOperationException("Variant not found");
        variant.Price = evt.Price;
        variant.DiscountPrice = evt.DiscountPrice;
    }

    private void Apply(VariationDimensionUpdatedEvent evt)
    {
        var dimension = VariationDimensions.FirstOrDefault(x => x.Name == evt.Name) ??
                        throw new InvalidOperationException("Dimension not found");
        dimension.DisplayName = evt.DisplayName;
    }

    private void Apply(VariationDimensionValuesChangedEvent evt)
    {
        var dimension = VariationDimensions.FirstOrDefault(x => x.Name == evt.Name) ??
                        throw new InvalidOperationException("Dimension not found");
        dimension.Values = evt.Values;
    }

    private void Apply(VariantPublishedEvent evt)
    {
        var variant = Variants.FirstOrDefault(x => x.Id == evt.VariantId) ??
                      throw new InvalidOperationException("Variant not found");
        variant.State = VariantStates.Published;

    }

    private void Apply(VariantUnpublishedEvent evt)
    {
        var variant = Variants.FirstOrDefault(x => x.Id == evt.VariantId) ??
                      throw new InvalidOperationException("Variant not found");
        variant.State = VariantStates.Unpublished;

    }

    #endregion

    public void ResetEvents()
    {
        throw new NotImplementedException();
    }
}