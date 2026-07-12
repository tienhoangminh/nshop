using nShop.Catalog.Aggregates;
using nShop.Catalog.Client.Abstractions.Dtos;
using nShop.Catalog.DomainEvents;

namespace nShop.Catalog.SyncService.MongoDb;

internal class CatalogEvent2DtoProfile : Profile
{
    public CatalogEvent2DtoProfile()
    {
        CreateMap<ProductCreatedEvent, ProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId)
            );

        CreateMap<CategoryCreatedEvent, CategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId));
        
        CreateMap<VariantCreatedEvent, ProductVariantDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.VariantId))
            .ForMember(dest => dest.DimensionValues, opt => opt.MapFrom(src => src.VariantDimensionValues));

        CreateMap<VariationDimensionAddedEvent, VariationDimensionDto>();
        CreateMap<VariantDimensionValue, ProductVariantValueDto>();
    }
}