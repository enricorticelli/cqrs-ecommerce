using Catalog.Application.Abstractions.Brands;
using Catalog.Application.Views;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Services.Common;
using Microsoft.EntityFrameworkCore;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Catalog;
using Shared.BuildingBlocks.Exceptions;
using Wolverine.EntityFrameworkCore;

namespace Catalog.Infrastructure.Services.Brands;

public sealed class BrandService(
    CatalogDbContext dbContext,
    IDbContextOutbox<CatalogDbContext> outbox) : CatalogServiceBase(dbContext, outbox), IBrandService
{
    public async Task<IReadOnlyList<BrandView>> GetBrandsAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var normalizedSearch = searchTerm?.Trim().ToLowerInvariant();
        var query = DbContext.Brands.AsQueryable();
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(normalizedSearch) ||
                x.Slug.ToLower().Contains(normalizedSearch) ||
                x.Description.ToLower().Contains(normalizedSearch));
        }

        return await query.OrderBy(x => x.Name)
            .Select(x => new BrandView(x.Id, x.Name, x.Slug, x.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<BrandView> GetBrandAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Brands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Brand '{id}' not found.");

        return new BrandView(entity.Id, entity.Name, entity.Slug, entity.Description);
    }

    public async Task<BrandView> CreateBrandAsync(string name, string slug, string description, string correlationId, CancellationToken cancellationToken)
    {
        await EnsureSlugUniqueAsync(slug, null, cancellationToken);

        var entity = new Brand
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Slug = slug.Trim(),
            Description = description.Trim()
        };

        DbContext.Brands.Add(entity);
        await PublishAndFlushAsync(new BrandCreatedV1(entity.Id, entity.Name, entity.Slug, CreateMetadata(correlationId)), cancellationToken);

        return new BrandView(entity.Id, entity.Name, entity.Slug, entity.Description);
    }

    public async Task<BrandView> UpdateBrandAsync(Guid id, string name, string slug, string description, string correlationId, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Brands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Brand '{id}' not found.");

        await EnsureSlugUniqueAsync(slug, id, cancellationToken);

        entity.Name = name.Trim();
        entity.Slug = slug.Trim();
        entity.Description = description.Trim();

        await PublishAndFlushAsync(new BrandUpdatedV1(entity.Id, entity.Name, entity.Slug, CreateMetadata(correlationId)), cancellationToken);

        return new BrandView(entity.Id, entity.Name, entity.Slug, entity.Description);
    }

    public async Task DeleteBrandAsync(Guid id, string correlationId, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Brands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Brand '{id}' not found.");

        var isReferenced = await DbContext.Products.AnyAsync(x => x.BrandId == id, cancellationToken);
        if (isReferenced)
        {
            throw new ConflictAppException($"Brand '{id}' is referenced by at least one product.");
        }

        DbContext.Brands.Remove(entity);
        await PublishAndFlushAsync(new BrandDeletedV1(id, CreateMetadata(correlationId)), cancellationToken);
    }

    private async Task EnsureSlugUniqueAsync(string slug, Guid? currentId, CancellationToken cancellationToken)
    {
        var exists = await DbContext.Brands.AnyAsync(x => x.Slug == slug && (!currentId.HasValue || x.Id != currentId.Value), cancellationToken);
        if (exists)
        {
            throw new ConflictAppException($"Brand slug '{slug}' already exists.");
        }
    }
}
