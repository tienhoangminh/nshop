using nShop.Catalog.IntegrationEvents;

namespace nShop.Catalog.Api.AutoMapperProfiles;

public class Aggregate2IntegrationEventProfile : Profile
{
    public Aggregate2IntegrationEventProfile() {
        CreateMap<CategoryCreatedEvent, CategoryCreatedIntegrationEvent>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Timestamp));

        CreateMap<Category, CategoryCreatedIntegrationEvent>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<CategoryUpdatedEvent, CategoryUpdatedIntegrationEvent>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));
        CreateMap<ProductCreatedEvent, ProductCreatedIntegrationEvent>();

        CreateMap<ProductUpdatedEvent, ProductUpdatedIntegrationEvent>();
        CreateMap<VariationDimensionAddedEvent, VariationDimensionAddedIntegrationEvent>();
        CreateMap<VariantPriceChangedEvent, VariantPriceChangedIntegrationEvent>();
        CreateMap<VariantUpdatedEvent, VariantUpdatedIntegrationEvent>();

        CreateMap<Aggregates.Variant, IntegrationEvents.Variant>();
        CreateMap<Product, ProductCreatedIntegrationEvent>();
        CreateMap<Product, ProductUpdatedIntegrationEvent>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.CreatedAt));
    }
}