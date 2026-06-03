namespace nShop.Catalog.Client.Abstractions.Dtos;

public class ProductDto : IDto
{
    public ProductDto()
    {
        VariationDimensions = [];
        Variants = [];
    }

    public ProductDto(Guid id, string name, string description, Guid categoryId, string[] tags, string slug, ProductStates state, string[] images, Guid[] groups, Guid tenantId)
    {
        Id = id;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        Tags = tags;
        Slug = slug;
        State = state;
        Images = images;
        Groups = groups;
        TenantId = tenantId;
        VariationDimensions = [];
        Variants = [];
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string[] Tags { get; set; } = [];
    public string Slug { get; set; } = string.Empty;
    public ProductStates State { get; set; } = ProductStates.Draft;
    public bool IsCategoryPublished { get; set; }
    public string[] Images { get; set; } = [];
    public Guid[] Groups { get; set; } = [];
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<VariationDimensionDto> VariationDimensions { get; set; }
    public List<ProductVariantDto> Variants { get; set; }
}

public enum ProductStates
{
    Draft = 1,
    Published = 2,
    Unpublished = 3,
    Deleted = 4
}