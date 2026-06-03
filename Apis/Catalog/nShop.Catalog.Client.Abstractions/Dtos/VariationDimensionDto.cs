using nShop.Catalog.DomainEvents;

namespace nShop.Catalog.Client.Abstractions.Dtos
{
    public class VariationDimensionDto
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public VariationDisplayStyles DisplayStyle { get; set; } = VariationDisplayStyles.Text;
        public string[] Values { get; set; } = [];
    }
}
