using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.ReadModels;

namespace Order.Infrastructure.Persistence.ReadModels;

public sealed class MongoOrderReadModelStore
    : MongoGuidReadModelStoreBase<OrderReadModelRow>, IOrderReadModelStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoOrderReadModelStore()
        : base("ORDER_READ_DB", "orderread", "order_read_models")
    {
        var connectionString = Shared.BuildingBlocks.Infrastructure.InfrastructureConnectionFactory.BuildMongoConnectionString();
        var databaseName = Environment.GetEnvironmentVariable("ORDER_READ_DB") ?? "orderread";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<BsonDocument>("order_read_models");
    }

    protected override string GetDocumentId(OrderReadModelRow model)
    {
        return model.OrderId.ToString("D");
    }

    protected override OrderReadModelRow MapToReadModel(Guid id, BsonDocument document)
    {
        var itemsJson = document["itemsJson"].AsString;
        var items = JsonSerializer.Deserialize<List<OrderItemDto>>(itemsJson, SerializerOptions) ?? [];
        var customer = document.TryGetValue("customerJson", out var customerJsonValue)
            ? JsonSerializer.Deserialize<OrderCustomerDetails>(customerJsonValue.AsString, SerializerOptions) ?? OrderCustomerDetails.Empty
            : OrderCustomerDetails.Empty;
        var shippingAddress = document.TryGetValue("shippingAddressJson", out var shippingAddressJsonValue)
            ? JsonSerializer.Deserialize<OrderAddress>(shippingAddressJsonValue.AsString, SerializerOptions) ?? OrderAddress.Empty
            : OrderAddress.Empty;
        var billingAddress = document.TryGetValue("billingAddressJson", out var billingAddressJsonValue)
            ? JsonSerializer.Deserialize<OrderAddress>(billingAddressJsonValue.AsString, SerializerOptions) ?? OrderAddress.Empty
            : OrderAddress.Empty;

        return new OrderReadModelRow(
            id,
            Guid.Parse(document["cartId"].AsString),
            Guid.Parse(document["userId"].AsString),
            document.TryGetValue("identityType", out var identityTypeValue)
                ? identityTypeValue.AsString
                : OrderIdentityTypes.Anonymous,
            document.TryGetValue("paymentMethod", out var paymentMethodValue)
                ? paymentMethodValue.AsString
                : PaymentMethodTypes.StripeCard,
            ParseOptionalGuid(document, "authenticatedUserId"),
            ParseOptionalGuid(document, "anonymousId"),
            customer,
            shippingAddress,
            billingAddress,
            document["status"].AsString,
            document["totalAmount"].ToDecimal(),
            items,
            document["transactionId"].AsString,
            document["trackingCode"].AsString,
            document["failureReason"].AsString);
    }

    protected override BsonDocument MapToDocument(OrderReadModelRow model)
    {
        return new BsonDocument
        {
            ["cartId"] = model.CartId.ToString("D"),
            ["userId"] = model.UserId.ToString("D"),
            ["identityType"] = model.IdentityType,
            ["paymentMethod"] = model.PaymentMethod,
            ["authenticatedUserId"] = model.AuthenticatedUserId?.ToString("D") ?? string.Empty,
            ["anonymousId"] = model.AnonymousId?.ToString("D") ?? string.Empty,
            ["customerJson"] = JsonSerializer.Serialize(model.Customer, SerializerOptions),
            ["shippingAddressJson"] = JsonSerializer.Serialize(model.ShippingAddress, SerializerOptions),
            ["billingAddressJson"] = JsonSerializer.Serialize(model.BillingAddress, SerializerOptions),
            ["status"] = model.Status,
            ["totalAmount"] = model.TotalAmount,
            ["itemsJson"] = JsonSerializer.Serialize(model.Items, SerializerOptions),
            ["transactionId"] = model.TransactionId,
            ["trackingCode"] = model.TrackingCode,
            ["failureReason"] = model.FailureReason
        };
    }

    public async Task<IReadOnlyList<OrderReadModelRow>> ListAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);

        var docs = await _collection
            .Find(Builders<BsonDocument>.Filter.Empty)
            .Sort(Builders<BsonDocument>.Sort.Descending("updatedAtUtc"))
            .Skip(safeOffset)
            .Limit(safeLimit)
            .ToListAsync(cancellationToken);

        var list = new List<OrderReadModelRow>(docs.Count);
        foreach (var doc in docs)
        {
            var id = Guid.Parse(doc["_id"].AsString);
            list.Add(MapToReadModel(id, doc));
        }

        return list;
    }

    private static Guid? ParseOptionalGuid(BsonDocument document, string fieldName)
    {
        if (!document.TryGetValue(fieldName, out var value))
        {
            return null;
        }

        var raw = value.IsString ? value.AsString : string.Empty;
        return Guid.TryParse(raw, out var parsed) ? parsed : null;
    }
}
