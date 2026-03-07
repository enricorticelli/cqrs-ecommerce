using MongoDB.Bson;
using MongoDB.Driver;
using Shared.BuildingBlocks.Infrastructure;

namespace Payment.Infrastructure.Persistence.ReadModels;

public sealed class MongoPaymentReadModelStore : IPaymentReadModelStore
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoPaymentReadModelStore()
    {
        var connectionString = InfrastructureConnectionFactory.BuildMongoConnectionString();
        var databaseName = Environment.GetEnvironmentVariable("PAYMENT_READ_DB") ?? "paymentread";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<BsonDocument>("payment_sessions");
    }

    public async Task<PaymentReadModelRow?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var id = sessionId.ToString("D");
        var doc = await _collection.Find(Builders<BsonDocument>.Filter.Eq("_id", id)).FirstOrDefaultAsync(cancellationToken);
        return doc is null ? null : MapToRow(doc);
    }

    public async Task<PaymentReadModelRow?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var orderIdText = orderId.ToString("D");
        var doc = await _collection.Find(Builders<BsonDocument>.Filter.Eq("orderId", orderIdText))
            .Sort(Builders<BsonDocument>.Sort.Descending("createdAtUtc"))
            .FirstOrDefaultAsync(cancellationToken);
        return doc is null ? null : MapToRow(doc);
    }

    public async Task<IReadOnlyList<PaymentReadModelRow>> ListAsync(int limit, CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var docs = await _collection.Find(Builders<BsonDocument>.Filter.Empty)
            .Sort(Builders<BsonDocument>.Sort.Descending("createdAtUtc"))
            .Limit(safeLimit)
            .ToListAsync(cancellationToken);

        return docs.Select(MapToRow).ToArray();
    }

    public Task UpsertAsync(PaymentReadModelRow model, CancellationToken cancellationToken)
    {
        var id = model.SessionId.ToString("D");
        var doc = new BsonDocument
        {
            ["_id"] = id,
            ["sessionId"] = id,
            ["orderId"] = model.OrderId.ToString("D"),
            ["userId"] = model.UserId.ToString("D"),
            ["amount"] = model.Amount,
            ["paymentMethod"] = model.PaymentMethod,
            ["status"] = model.Status,
            ["transactionId"] = model.TransactionId is null ? BsonNull.Value : model.TransactionId,
            ["failureReason"] = model.FailureReason is null ? BsonNull.Value : model.FailureReason,
            ["createdAtUtc"] = model.CreatedAtUtc.UtcDateTime,
            ["completedAtUtc"] = model.CompletedAtUtc is null ? BsonNull.Value : model.CompletedAtUtc.Value.UtcDateTime
        };

        return _collection.ReplaceOneAsync(
            Builders<BsonDocument>.Filter.Eq("_id", id),
            doc,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    private static PaymentReadModelRow MapToRow(BsonDocument doc)
    {
        var completedAtUtc = doc.TryGetValue("completedAtUtc", out var completedRaw) && !completedRaw.IsBsonNull
            ? completedRaw.ToUniversalTime()
            : (DateTimeOffset?)null;

        var transactionId = doc.TryGetValue("transactionId", out var transactionRaw) && !transactionRaw.IsBsonNull
            ? transactionRaw.AsString
            : null;

        var failureReason = doc.TryGetValue("failureReason", out var failureRaw) && !failureRaw.IsBsonNull
            ? failureRaw.AsString
            : null;

        return new PaymentReadModelRow(
            Guid.Parse(doc["sessionId"].AsString),
            Guid.Parse(doc["orderId"].AsString),
            Guid.Parse(doc["userId"].AsString),
            ReadDecimal(doc["amount"]),
            doc["paymentMethod"].AsString,
            doc["status"].AsString,
            transactionId,
            failureReason,
            ReadDateTimeOffset(doc["createdAtUtc"]),
            completedAtUtc);
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

    private static DateTimeOffset ReadDateTimeOffset(BsonValue value)
    {
        if (value.IsBsonDateTime)
        {
            return value.ToUniversalTime();
        }

        return DateTimeOffset.Parse(value.AsString, System.Globalization.CultureInfo.InvariantCulture);
    }
}
