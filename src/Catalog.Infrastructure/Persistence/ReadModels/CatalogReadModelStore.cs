using MongoDB.Bson;
using MongoDB.Driver;
using Shared.BuildingBlocks.Infrastructure;

namespace Catalog.Infrastructure.Persistence.ReadModels;

public sealed class CatalogReadModelStore
{
    private readonly IMongoCollection<BsonDocument> _brands;
    private readonly IMongoCollection<BsonDocument> _categories;
    private readonly IMongoCollection<BsonDocument> _collections;
    private readonly IMongoCollection<BsonDocument> _products;

    public CatalogReadModelStore()
    {
        var client = new MongoClient(InfrastructureConnectionFactory.BuildMongoConnectionString());
        var databaseName = Environment.GetEnvironmentVariable("CATALOG_READ_DB") ?? "catalogread";
        var db = client.GetDatabase(databaseName);
        _brands = db.GetCollection<BsonDocument>("brands");
        _categories = db.GetCollection<BsonDocument>("categories");
        _collections = db.GetCollection<BsonDocument>("collections");
        _products = db.GetCollection<BsonDocument>("products");
    }

    public Task UpsertBrandAsync(Guid id, string name, string slug, string description, bool isDeleted, CancellationToken cancellationToken)
        => UpsertAsync(_brands, id, new BsonDocument
        {
            ["name"] = name,
            ["slug"] = slug,
            ["description"] = description,
            ["isDeleted"] = isDeleted
        }, cancellationToken);

    public Task UpsertCategoryAsync(Guid id, string name, string slug, string description, bool isDeleted, CancellationToken cancellationToken)
        => UpsertAsync(_categories, id, new BsonDocument
        {
            ["name"] = name,
            ["slug"] = slug,
            ["description"] = description,
            ["isDeleted"] = isDeleted
        }, cancellationToken);

    public Task UpsertCollectionAsync(Guid id, string name, string slug, string description, bool isFeatured, bool isDeleted, CancellationToken cancellationToken)
        => UpsertAsync(_collections, id, new BsonDocument
        {
            ["name"] = name,
            ["slug"] = slug,
            ["description"] = description,
            ["isFeatured"] = isFeatured,
            ["isDeleted"] = isDeleted
        }, cancellationToken);

    public Task UpsertProductAsync(
        Guid id,
        string sku,
        string name,
        string description,
        decimal price,
        Guid brandId,
        Guid categoryId,
        IReadOnlyList<Guid> collectionIds,
        bool isNewArrival,
        bool isBestSeller,
        DateTimeOffset createdAtUtc,
        bool isDeleted,
        CancellationToken cancellationToken)
        => UpsertAsync(_products, id, new BsonDocument
        {
            ["sku"] = sku,
            ["name"] = name,
            ["description"] = description,
            ["price"] = price,
            ["brandId"] = brandId.ToString("D"),
            ["categoryId"] = categoryId.ToString("D"),
            ["collectionIds"] = new BsonArray(collectionIds.Select(x => x.ToString("D"))),
            ["isNewArrival"] = isNewArrival,
            ["isBestSeller"] = isBestSeller,
            ["createdAtUtc"] = createdAtUtc.UtcDateTime,
            ["isDeleted"] = isDeleted
        }, cancellationToken);

    public async Task<IReadOnlyList<BrandReadModelRow>> ListBrandsAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var docs = await FindAsync(_brands, limit, offset, searchTerm, cancellationToken);
        return docs.Select(ToBrand).Where(x => !x.IsDeleted).ToArray();
    }

    public async Task<BrandReadModelRow?> GetBrandByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var doc = await GetByIdAsync(_brands, id, cancellationToken);
        if (doc is null)
        {
            return null;
        }

        var row = ToBrand(doc);
        return row.IsDeleted ? null : row;
    }

    public async Task<IReadOnlyList<CategoryReadModelRow>> ListCategoriesAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var docs = await FindAsync(_categories, limit, offset, searchTerm, cancellationToken);
        return docs.Select(ToCategory).Where(x => !x.IsDeleted).ToArray();
    }

    public async Task<CategoryReadModelRow?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var doc = await GetByIdAsync(_categories, id, cancellationToken);
        if (doc is null)
        {
            return null;
        }

        var row = ToCategory(doc);
        return row.IsDeleted ? null : row;
    }

    public async Task<IReadOnlyList<CollectionReadModelRow>> ListCollectionsAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var docs = await FindAsync(_collections, limit, offset, searchTerm, cancellationToken);
        return docs.Select(ToCollection).Where(x => !x.IsDeleted).ToArray();
    }

    public async Task<CollectionReadModelRow?> GetCollectionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var doc = await GetByIdAsync(_collections, id, cancellationToken);
        if (doc is null)
        {
            return null;
        }

        var row = ToCollection(doc);
        return row.IsDeleted ? null : row;
    }

    public async Task<IReadOnlyList<ProductReadModelRow>> ListProductsAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var docs = await FindAsync(_products, limit, offset, searchTerm, cancellationToken);
        return docs.Select(ToProduct).Where(x => !x.IsDeleted).ToArray();
    }

    public async Task<ProductReadModelRow?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var doc = await GetByIdAsync(_products, id, cancellationToken);
        if (doc is null)
        {
            return null;
        }

        var row = ToProduct(doc);
        return row.IsDeleted ? null : row;
    }

    public async Task<IReadOnlyList<ProductReadModelRow>> ListNewArrivalsAsync(CancellationToken cancellationToken)
    {
        var docs = await _products.Find(Builders<BsonDocument>.Filter.Eq("isNewArrival", true))
            .ToListAsync(cancellationToken);
        return docs.Select(ToProduct).Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedAtUtc).ToArray();
    }

    public async Task<IReadOnlyList<ProductReadModelRow>> ListBestSellersAsync(CancellationToken cancellationToken)
    {
        var docs = await _products.Find(Builders<BsonDocument>.Filter.Eq("isBestSeller", true))
            .ToListAsync(cancellationToken);
        return docs.Select(ToProduct).Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToArray();
    }

    public async Task<IDictionary<Guid, BrandReadModelRow>> GetBrandsByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var keyIds = ids.Distinct().ToArray();
        if (keyIds.Length == 0)
        {
            return new Dictionary<Guid, BrandReadModelRow>();
        }

        var idTexts = keyIds.Select(x => x.ToString("D")).ToArray();
        var docs = await _brands.Find(Builders<BsonDocument>.Filter.In("_id", idTexts)).ToListAsync(cancellationToken);
        return docs.Select(ToBrand).Where(x => !x.IsDeleted).ToDictionary(x => x.Id, x => x);
    }

    public async Task<IDictionary<Guid, CategoryReadModelRow>> GetCategoriesByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var keyIds = ids.Distinct().ToArray();
        if (keyIds.Length == 0)
        {
            return new Dictionary<Guid, CategoryReadModelRow>();
        }

        var idTexts = keyIds.Select(x => x.ToString("D")).ToArray();
        var docs = await _categories.Find(Builders<BsonDocument>.Filter.In("_id", idTexts)).ToListAsync(cancellationToken);
        return docs.Select(ToCategory).Where(x => !x.IsDeleted).ToDictionary(x => x.Id, x => x);
    }

    public async Task<IDictionary<Guid, CollectionReadModelRow>> GetCollectionsByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var keyIds = ids.Distinct().ToArray();
        if (keyIds.Length == 0)
        {
            return new Dictionary<Guid, CollectionReadModelRow>();
        }

        var idTexts = keyIds.Select(x => x.ToString("D")).ToArray();
        var docs = await _collections.Find(Builders<BsonDocument>.Filter.In("_id", idTexts)).ToListAsync(cancellationToken);
        return docs.Select(ToCollection).Where(x => !x.IsDeleted).ToDictionary(x => x.Id, x => x);
    }

    private static async Task UpsertAsync(IMongoCollection<BsonDocument> collection, Guid id, BsonDocument payload, CancellationToken cancellationToken)
    {
        var docId = id.ToString("D");
        payload["_id"] = docId;
        await collection.ReplaceOneAsync(
            Builders<BsonDocument>.Filter.Eq("_id", docId),
            payload,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    private static async Task<IReadOnlyList<BsonDocument>> FindAsync(
        IMongoCollection<BsonDocument> collection,
        int limit,
        int offset,
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);
        var filter = string.IsNullOrWhiteSpace(searchTerm)
            ? Builders<BsonDocument>.Filter.Empty
            : Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Regex("name", new BsonRegularExpression(searchTerm, "i")),
                Builders<BsonDocument>.Filter.Regex("slug", new BsonRegularExpression(searchTerm, "i")),
                Builders<BsonDocument>.Filter.Regex("description", new BsonRegularExpression(searchTerm, "i")),
                Builders<BsonDocument>.Filter.Regex("_id", new BsonRegularExpression(searchTerm, "i")));

        return await collection.Find(filter)
            .Skip(safeOffset)
            .Limit(safeLimit)
            .ToListAsync(cancellationToken);
    }

    private static async Task<BsonDocument?> GetByIdAsync(IMongoCollection<BsonDocument> collection, Guid id, CancellationToken cancellationToken)
    {
        var docId = id.ToString("D");
        return await collection.Find(Builders<BsonDocument>.Filter.Eq("_id", docId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static BrandReadModelRow ToBrand(BsonDocument doc)
        => new(
            Guid.Parse(doc["_id"].AsString),
            doc.GetValue("name", string.Empty).AsString,
            doc.GetValue("slug", string.Empty).AsString,
            doc.GetValue("description", string.Empty).AsString,
            doc.GetValue("isDeleted", false).AsBoolean);

    private static CategoryReadModelRow ToCategory(BsonDocument doc)
        => new(
            Guid.Parse(doc["_id"].AsString),
            doc.GetValue("name", string.Empty).AsString,
            doc.GetValue("slug", string.Empty).AsString,
            doc.GetValue("description", string.Empty).AsString,
            doc.GetValue("isDeleted", false).AsBoolean);

    private static CollectionReadModelRow ToCollection(BsonDocument doc)
        => new(
            Guid.Parse(doc["_id"].AsString),
            doc.GetValue("name", string.Empty).AsString,
            doc.GetValue("slug", string.Empty).AsString,
            doc.GetValue("description", string.Empty).AsString,
            doc.GetValue("isFeatured", false).AsBoolean,
            doc.GetValue("isDeleted", false).AsBoolean);

    private static ProductReadModelRow ToProduct(BsonDocument doc)
    {
        var collectionIds = doc.TryGetValue("collectionIds", out var collectionIdsValue) && collectionIdsValue.IsBsonArray
            ? collectionIdsValue.AsBsonArray.Select(x => Guid.Parse(x.AsString)).ToArray()
            : Array.Empty<Guid>();

        return new ProductReadModelRow(
            Guid.Parse(doc["_id"].AsString),
            doc.GetValue("sku", string.Empty).AsString,
            doc.GetValue("name", string.Empty).AsString,
            doc.GetValue("description", string.Empty).AsString,
            ReadDecimal(doc.GetValue("price", BsonDecimal128.Create(0m))),
            Guid.Parse(doc.GetValue("brandId", Guid.Empty.ToString("D")).AsString),
            Guid.Parse(doc.GetValue("categoryId", Guid.Empty.ToString("D")).AsString),
            collectionIds,
            doc.GetValue("isNewArrival", false).AsBoolean,
            doc.GetValue("isBestSeller", false).AsBoolean,
            doc.GetValue("createdAtUtc", BsonDateTime.Create(DateTimeOffset.UnixEpoch.UtcDateTime)).ToUniversalTime(),
            doc.GetValue("isDeleted", false).AsBoolean);
    }

    private static decimal ReadDecimal(BsonValue value)
    {
        if (value.IsDecimal128)
        {
            return (decimal)value.AsDecimal128;
        }

        if (value.IsDouble)
        {
            return Convert.ToDecimal(value.AsDouble);
        }

        if (value.IsInt32)
        {
            return value.AsInt32;
        }

        if (value.IsInt64)
        {
            return value.AsInt64;
        }

        return decimal.Parse(value.AsString, System.Globalization.CultureInfo.InvariantCulture);
    }
}

public sealed record BrandReadModelRow(Guid Id, string Name, string Slug, string Description, bool IsDeleted);

public sealed record CategoryReadModelRow(Guid Id, string Name, string Slug, string Description, bool IsDeleted);

public sealed record CollectionReadModelRow(Guid Id, string Name, string Slug, string Description, bool IsFeatured, bool IsDeleted);

public sealed record ProductReadModelRow(
    Guid Id,
    string Sku,
    string Name,
    string Description,
    decimal Price,
    Guid BrandId,
    Guid CategoryId,
    IReadOnlyList<Guid> CollectionIds,
    bool IsNewArrival,
    bool IsBestSeller,
    DateTimeOffset CreatedAtUtc,
    bool IsDeleted);
