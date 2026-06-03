namespace nShop.Catalog.DomainEvents;

public class VariantPriceChangedEvent : VariantEvent
{
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
}