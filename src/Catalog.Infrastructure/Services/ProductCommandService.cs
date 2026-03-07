using Catalog.Application.Abstractions;
using Catalog.Application.Products;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Catalog.Domain.Events;
using Catalog.Domain.Events.Product;
using Marten;

namespace Catalog.Infrastructure.Services;

public sealed class ProductCommandService(
    IQuerySession querySession,
    IDocumentSession documentSession) : IProductCommandService
{
    public async Task<ProductView?> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var hasValidReferences = await ProductViewBuilder.ReferencesExistAsync(
            querySession,
            command.BrandId,
            command.CategoryId,
            command.CollectionIds,
            cancellationToken);

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
        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);

        return await ProductViewBuilder.BuildProductViewAsync(querySession, state, cancellationToken);
    }

    public async Task<ProductView?> UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var state = await documentSession.LoadAsync<ProductAggregate>(id, cancellationToken);
        if (state is null || state.IsDeleted)
        {
            return null;
        }

        var hasValidReferences = await ProductViewBuilder.ReferencesExistAsync(
            querySession,
            command.BrandId,
            command.CategoryId,
            command.CollectionIds,
            cancellationToken);

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

        documentSession.Events.Append(id, @event);
        state.Apply(@event);

        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);

        return await ProductViewBuilder.BuildProductViewAsync(querySession, state, cancellationToken);
    }

    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var state = await documentSession.LoadAsync<ProductAggregate>(id, cancellationToken);
        if (state is null || state.IsDeleted)
        {
            return false;
        }

        var @event = new ProductDeletedDomainEvent(id);
        documentSession.Events.Append(id, @event);
        state.Apply(@event);

        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }
}
