using Catalog.Application.Abstractions;
using Catalog.Application.Products;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Catalog.Domain.Events.Product;
using Marten;
using Wolverine;

namespace Catalog.Infrastructure.Services;

public sealed class ProductCommandService(
    IBrandQueryService brandQueryService,
    ICategoryQueryService categoryQueryService,
    ICollectionQueryService collectionQueryService,
    IProductQueryService productQueryService,
    IDocumentSession documentSession,
    IMessageBus bus) : IProductCommandService
{
    public async Task<ProductView?> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var hasValidReferences = await ReferencesExistAsync(command.BrandId, command.CategoryId, command.CollectionIds, cancellationToken);

        if (!hasValidReferences)
        {
            return null;
        }

        var productId = Guid.NewGuid();
        var createdAtUtc = DateTimeOffset.UtcNow;
        var @event = new ProductCreatedDomainEvent(
            productId,
            command.Sku,
            command.Name,
            command.Description,
            command.Price,
            command.BrandId,
            command.CategoryId,
            command.CollectionIds,
            command.IsNewArrival,
            command.IsBestSeller,
            createdAtUtc);

        var state = new ProductAggregate();
        state.Apply(@event);

        documentSession.Events.StartStream<ProductAggregate>(productId, @event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event, cancellationToken);

        return await productQueryService.GetProductByIdAsync(productId, cancellationToken);
    }

    public async Task<ProductView?> UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<ProductAggregate>(id, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsDeleted == true)
        {
            return null;
        }

        var hasValidReferences = await ReferencesExistAsync(command.BrandId, command.CategoryId, command.CollectionIds, cancellationToken);

        if (!hasValidReferences)
        {
            return null;
        }

        var @event = new ProductUpdatedDomainEvent(
            id,
            command.Sku,
            command.Name,
            command.Description,
            command.Price,
            command.BrandId,
            command.CategoryId,
            command.CollectionIds,
            command.IsNewArrival,
            command.IsBestSeller);

        stream.AppendOne(@event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event, cancellationToken);

        return await productQueryService.GetProductByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<ProductAggregate>(id, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsDeleted == true)
        {
            return false;
        }

        var @event = new ProductDeletedDomainEvent(id);
        stream.AppendOne(@event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event, cancellationToken);
        return true;
    }

    private async Task<bool> ReferencesExistAsync(
        Guid brandId,
        Guid categoryId,
        IReadOnlyList<Guid> collectionIds,
        CancellationToken cancellationToken)
    {
        var brand = await brandQueryService.GetBrandByIdAsync(brandId, cancellationToken);
        var category = await categoryQueryService.GetCategoryByIdAsync(categoryId, cancellationToken);
        if (brand is null || category is null)
        {
            return false;
        }

        if (collectionIds.Count == 0)
        {
            return true;
        }

        var distinctCollectionIds = collectionIds.Distinct().ToArray();
        foreach (var collectionId in distinctCollectionIds)
        {
            var collection = await collectionQueryService.GetCollectionByIdAsync(collectionId, cancellationToken);
            if (collection is null)
            {
                return false;
            }
        }

        return true;
    }
}
