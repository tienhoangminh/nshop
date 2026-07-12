namespace nShop.Catalog.Aggregates;

public class VariationDimension
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public VariationDisplayStyles DisplayStyle { get; set; } = VariationDisplayStyles.Text;
    public string[] Values { get; set; } = [];
}