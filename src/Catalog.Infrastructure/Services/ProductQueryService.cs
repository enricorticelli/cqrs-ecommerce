using Catalog.Application.Abstractions;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Marten;

namespace Catalog.Infrastructure.Services;

public sealed class ProductQueryService(IQuerySession querySession) : IProductQueryService
{
    public async Task<IReadOnlyList<ProductView>> GetProductsAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);

        var products = await querySession.Query<ProductAggregate>()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Skip(safeOffset)
            .Take(safeLimit)
            .ToListAsync(cancellationToken);

        return await ProductViewBuilder.BuildProductViewsAsync(querySession, products, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductView>> GetNewArrivalsAsync(CancellationToken cancellationToken)
    {
        var products = await querySession.Query<ProductAggregate>()
            .Where(x => x.IsNewArrival && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return await ProductViewBuilder.BuildProductViewsAsync(querySession, products, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductView>> GetBestSellersAsync(CancellationToken cancellationToken)
    {
        var products = await querySession.Query<ProductAggregate>()
            .Where(x => x.IsBestSeller && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return await ProductViewBuilder.BuildProductViewsAsync(querySession, products, cancellationToken);
    }

    public async Task<ProductView?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await querySession.LoadAsync<ProductAggregate>(id, cancellationToken);
        if (product is null || product.IsDeleted)
        {
            return null;
        }

        return await ProductViewBuilder.BuildProductViewAsync(querySession, product, cancellationToken);
    }
}
