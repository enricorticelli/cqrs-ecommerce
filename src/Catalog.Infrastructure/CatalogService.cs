using Catalog.Application;
using Catalog.Domain;
using Marten;

namespace Catalog.Infrastructure;

public sealed class CatalogService(IQuerySession querySession, IDocumentSession documentSession) : ICatalogService
{
    public async Task<IReadOnlyList<ProductDocument>> GetProductsAsync(CancellationToken cancellationToken)
    {
        return await querySession.Query<ProductDocument>()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<ProductDocument?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return querySession.LoadAsync<ProductDocument>(id, cancellationToken);
    }

    public async Task<ProductDocument> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new ProductDocument
        {
            Id = Guid.NewGuid(),
            Sku = command.Sku,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price
        };

        documentSession.Store(product);
        await documentSession.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<ProductDocument?> UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var existing = await documentSession.LoadAsync<ProductDocument>(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var updated = new ProductDocument
        {
            Id = id,
            Sku = command.Sku,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price
        };

        documentSession.Store(updated);
        await documentSession.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var existing = await documentSession.LoadAsync<ProductDocument>(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        documentSession.Delete<ProductDocument>(id);
        await documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }
}
