namespace nShop.Catalog.Client.Abstractions.Dtos;

public class CategoryDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public CategoryStates State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum CategoryStates
{
    Unpublished = 0,
    Published = 1,
}