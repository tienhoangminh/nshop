using MongoDB.Driver.Linq;
using nShop.Catalog.Client.Abstractions.Dtos;
using nShop.Catalog.DomainEvents;

namespace nShop.Catalog.SyncService.MongoDb;

public class MongoDbReadModelSyncEventHandler : IReadModelSyncEventHandler
{
    private readonly IMongoClient mongoDbClient;
    private readonly IMapper mapper;
    private readonly ILogger<MongoDbReadModelSyncEventHandler> logger;

    private readonly IMongoCollection<CategoryDto> categoryCollection;
    private readonly IMongoCollection<ProductDto> productCollection;
    
    public MongoDbReadModelSyncEventHandler(IMongoClient mongoDbClient, string database, IMapper mapper, ILogger<MongoDbReadModelSyncEventHandler> logger)
    {
        this.mongoDbClient = mongoDbClient;
        this.mapper = mapper;
        this.logger = logger;
        
        var db = mongoDbClient.GetDatabase(database);

        categoryCollection = db.GetCollection<CategoryDto>(nameof(CategoryDto));
        productCollection = db.GetCollection<ProductDto>(nameof(ProductDto));
    }
    
    public Task HandleAsync(ICatalogEvent @event)
    {
        logger.LogInformation("Handling event {@event}", @event);

        return @event switch
        {
            CategoryCreatedEvent categoryCreatedEvent => CreateCategoryAsync(categoryCreatedEvent),
            CategoryPublishedEvent categoryPublishedEvent => PublishCategoryAsync(categoryPublishedEvent),
            CategoryUnpublishedEvent categoryUnpublishedEvent => UnpublishCategoryAsync(categoryUnpublishedEvent),
            CategoryUpdatedEvent categoryUpdatedEvent => UpdateCategoryAsync(categoryUpdatedEvent),
            ProductUpdatedEvent productUpdatedEvent => UpdateProductAsync(productUpdatedEvent),
            ProductCreatedEvent productCreatedEvent => CreateProductAsync(productCreatedEvent),
            ProductPublishedEvent productPublishedEvent => PublishProductAsync(productPublishedEvent),
            ProductUnpublishedEvent productUnpublishedEvent => UnpublishProductAsync(productUnpublishedEvent),
            VariationDimensionAddedEvent variationDimensionAddedEvent => AddVariationDimensionAsync(variationDimensionAddedEvent),
            VariantCreatedEvent variantCreatedEvent => CreateVariant(variantCreatedEvent),
            VariantUpdatedEvent variantUpdatedEvent => UpdateVariant(variantUpdatedEvent),
            VariantPriceChangedEvent variantPriceChangedEvent => ChangeVariantPrice(variantPriceChangedEvent),
            _ => throw new InvalidOperationException($"Unhandled event: {@event}"),
        };
    }
    
    private async Task UnpublishCategoryAsync(CategoryUnpublishedEvent evt)
    {
        try
        {
            logger.LogInformation("Change state of Category '{id}' to Unpublished", evt.CategoryId);

            await categoryCollection.UpdateOneAsync(p => p.Id == evt.CategoryId, 
                Builders<CategoryDto>.Update
                    .Set(p => p.State, Client.Abstractions.Dtos.CategoryStates.Unpublished));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while unpublishing category");
        }
    }
    
    private async Task PublishCategoryAsync(CategoryPublishedEvent evt)
    {
        try
        {
            logger.LogInformation("Change state of Category '{id}' to Published", evt.CategoryId);

            await categoryCollection.UpdateOneAsync(p => p.Id == evt.CategoryId, 
                Builders<CategoryDto>.Update
                    .Set(p => p.State, Client.Abstractions.Dtos.CategoryStates.Published));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while publishing category");
        }
    }
    
    private async Task UpdateCategoryAsync(CategoryUpdatedEvent evt)
    {
        try
        {
            logger.LogInformation("Update category: {id}", evt.CategoryId);
            await categoryCollection.UpdateOneAsync(p => p.Id == evt.CategoryId,
                Builders<CategoryDto>.Update
                    .Set(p => p.Name, evt.Name)
                    .Set(p => p.Slug, evt.Slug)
                    .Set(p => p.ParentId, evt.ParentId)
                    .Set(p => p.UpdatedAt, evt.Timestamp)
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while updating category");
        }
    }
    
    private async Task CreateCategoryAsync(CategoryCreatedEvent evt)
    {
        try
        {
            var category = mapper.Map<CategoryDto>(evt);

            logger.LogInformation("Create category: {id}", category.Id);
            await categoryCollection.InsertOneAsync(category);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while creating category");
        }
    }
    
    private async Task UpdateVariant(VariantUpdatedEvent evt)
    {
        try
        {
            logger.LogInformation("Update variant '{id}'", evt.VariantId);

            var filter = Builders<ProductDto>.Filter.Eq(x => x.Id, evt.ProductId)
                         & Builders<ProductDto>.Filter.ElemMatch(x => x.Variants, Builders<ProductVariantDto>.Filter.Eq(x => x.Id, evt.VariantId));

            var update = Builders<ProductDto>.Update
                .Set(x => x.Variants.FirstMatchingElement().Name, evt.Name)
                .Set(x => x.Variants.FirstMatchingElement().Sku, evt.Sku);

            var result = await productCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new SyncHandlerException($"Product '{evt.ProductId}' not found");
            }

            if (result.ModifiedCount == 0)
            {
                throw new SyncHandlerException($"Variant '{evt.VariantId}' not updated");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while updating variant price");
        }
    }
    
    private async Task ChangeVariantPrice(VariantPriceChangedEvent evt)
    {
        try
        {
            logger.LogInformation("Update variant price '{id}' to: {p}, discount price: {dp}", evt.VariantId, evt.Price, evt.DiscountPrice);

            var filter = Builders<ProductDto>.Filter.Eq(x => x.Id, evt.ProductId)
                         & Builders<ProductDto>.Filter.ElemMatch(x => x.Variants, Builders<ProductVariantDto>.Filter.Eq(x => x.Id, evt.VariantId));

            var update = Builders<ProductDto>.Update
                .Set(x => x.Variants.FirstMatchingElement().Price, evt.Price)
                .Set(x => x.Variants.FirstMatchingElement().DiscountPrice, evt.DiscountPrice);
            
            var result = await productCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new SyncHandlerException($"Product '{evt.ProductId}' not found");
            }

            if (result.ModifiedCount == 0)
            {
                throw new SyncHandlerException($"Variant '{evt.VariantId}' not updated");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while updating variant price");
        }
    }
    
    private async Task CreateVariant(VariantCreatedEvent evt)
    {
        try
        {
            logger.LogInformation("Add variant {id} to Product '{pid}'", evt.VariantId, evt.ProductId);

            //var product = (await productCollection.FindAsync(p => p.Id == evt.ProductId)).FirstOrDefault();
            //product.Variants.Add(mapper.Map<ProductVariantDto>(evt));

            var result = await productCollection.UpdateOneAsync(p => p.Id == evt.ProductId, 
                Builders<ProductDto>.Update
                    .AddToSet(p => p.Variants, mapper.Map<ProductVariantDto>(evt)));

            if (result.MatchedCount == 0)
            {
                throw new SyncHandlerException($"Product '{evt.ProductId}' not found");
            }

            if (result.ModifiedCount == 0)
            {
                throw new SyncHandlerException($"Variant '{evt.VariantId}' not added to Product '{evt.ProductId}'");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while adding Variants");
        }
    }
    
    private async Task AddVariationDimensionAsync(VariationDimensionAddedEvent evt)
    {
        try
        {
            logger.LogInformation("Add VariationDimension to Product '{id}'", evt.ProductId);

            //var product = (await productCollection.FindAsync(p => p.Id == evt.ProductId)).FirstOrDefault();
            //product.VariationDimensions.Add(mapper.Map<VariationDimensionDto>(evt));

            var result = await productCollection.UpdateOneAsync(p => p.Id == evt.ProductId, 
                Builders<ProductDto>.Update.AddToSet(p => p.VariationDimensions, mapper.Map<VariationDimensionDto>(evt))
            );

            if (result.MatchedCount == 0)
            {
                throw new SyncHandlerException($"Product '{evt.ProductId}' not found");
            }

            if (result.ModifiedCount == 0)
            {
                throw new SyncHandlerException($"VariationDimension '{evt.Name}' not added to Product '{evt.ProductId}'");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while adding VariationDimension");
        }
    }
    
    private async Task PublishProductAsync(ProductPublishedEvent evt)
    {
        try
        {
            logger.LogInformation("Change state of Product '{id}' to Published", evt.ProductId);

            await productCollection.UpdateOneAsync(p => p.Id == evt.ProductId, 
                Builders<ProductDto>.Update
                    .Set(p => p.State, Client.Abstractions.Dtos.ProductStates.Published));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while publishing product");
        }
    }
    
    private async Task UnpublishProductAsync(ProductUnpublishedEvent evt)
    {
        try
        {
            logger.LogInformation("Change state of Product '{id}' to Unpublished", evt.ProductId);

            await productCollection.UpdateOneAsync(p => p.Id == evt.ProductId, 
                Builders<ProductDto>.Update
                    .Set(p => p.State, Client.Abstractions.Dtos.ProductStates.Unpublished));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while unpublishing product");
        }
    }
    
    private async Task CreateProductAsync(ProductCreatedEvent evt)
    {
        try
        {
            var product = mapper.Map<ProductDto>(evt);

            logger.LogInformation("Create product: {id}", product.Id);
            await productCollection.InsertOneAsync(product);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error while creating product");
        }
    }
    
    private async Task UpdateProductAsync(ProductUpdatedEvent evt)
    {
        try
        {
            logger.LogInformation("Update product: {id}", evt.ProductId);
            await productCollection.UpdateOneAsync(p => p.Id == evt.ProductId,
                Builders<ProductDto>.Update
                    .Set(p => p.Name, evt.Name)
                    .Set(p => p.Slug, evt.Slug)
                    .Set(p => p.Description, evt.Description)
                    .Set(p => p.CategoryId, evt.CategoryId)
                    .Set(p => p.Tags, evt.Tags)
                    .Set(p => p.Images, evt.Images)
                    .Set(p => p.Groups, evt.Groups)
                    .Set(p => p.UpdatedAt, evt.Timestamp)
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while update product");
        }
    }
}