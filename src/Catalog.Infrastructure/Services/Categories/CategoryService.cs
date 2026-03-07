using Catalog.Application.Abstractions.Categories;
using Catalog.Application.Views;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Services.Common;
using Microsoft.EntityFrameworkCore;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Catalog;
using Shared.BuildingBlocks.Exceptions;
using Wolverine.EntityFrameworkCore;

namespace Catalog.Infrastructure.Services.Categories;

public sealed class CategoryService(
    CatalogDbContext dbContext,
    IDbContextOutbox<CatalogDbContext> outbox) : CatalogServiceBase(dbContext, outbox), ICategoryService
{
    public async Task<IReadOnlyList<CategoryView>> GetCategoriesAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var normalizedSearch = searchTerm?.Trim().ToLowerInvariant();
        var query = DbContext.Categories.AsQueryable();
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(normalizedSearch) ||
                x.Slug.ToLower().Contains(normalizedSearch) ||
                x.Description.ToLower().Contains(normalizedSearch));
        }

        return await query.OrderBy(x => x.Name)
            .Select(x => new CategoryView(x.Id, x.Name, x.Slug, x.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryView> GetCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Category '{id}' not found.");

        return new CategoryView(entity.Id, entity.Name, entity.Slug, entity.Description);
    }

    public async Task<CategoryView> CreateCategoryAsync(string name, string slug, string description, string correlationId, CancellationToken cancellationToken)
    {
        await EnsureSlugUniqueAsync(slug, null, cancellationToken);

        var entity = new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Slug = slug.Trim(),
            Description = description.Trim()
        };

        DbContext.Categories.Add(entity);
        await PublishAndFlushAsync(new CategoryCreatedV1(entity.Id, entity.Name, entity.Slug, CreateMetadata(correlationId)), cancellationToken);

        return new CategoryView(entity.Id, entity.Name, entity.Slug, entity.Description);
    }

    public async Task<CategoryView> UpdateCategoryAsync(Guid id, string name, string slug, string description, string correlationId, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Category '{id}' not found.");

        await EnsureSlugUniqueAsync(slug, id, cancellationToken);

        entity.Name = name.Trim();
        entity.Slug = slug.Trim();
        entity.Description = description.Trim();

        await PublishAndFlushAsync(new CategoryUpdatedV1(entity.Id, entity.Name, entity.Slug, CreateMetadata(correlationId)), cancellationToken);

        return new CategoryView(entity.Id, entity.Name, entity.Slug, entity.Description);
    }

    public async Task DeleteCategoryAsync(Guid id, string correlationId, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Category '{id}' not found.");

        var isReferenced = await DbContext.Products.AnyAsync(x => x.CategoryId == id, cancellationToken);
        if (isReferenced)
        {
            throw new ConflictAppException($"Category '{id}' is referenced by at least one product.");
        }

        DbContext.Categories.Remove(entity);
        await PublishAndFlushAsync(new CategoryDeletedV1(id, CreateMetadata(correlationId)), cancellationToken);
    }

    private async Task EnsureSlugUniqueAsync(string slug, Guid? currentId, CancellationToken cancellationToken)
    {
        var exists = await DbContext.Categories.AnyAsync(x => x.Slug == slug && (!currentId.HasValue || x.Id != currentId.Value), cancellationToken);
        if (exists)
        {
            throw new ConflictAppException($"Category slug '{slug}' already exists.");
        }
    }
}
