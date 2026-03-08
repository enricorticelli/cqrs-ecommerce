using Order.Domain.ValueObjects;
using Shared.BuildingBlocks.Exceptions;

namespace Order.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid UserId { get; private set; }
    public string IdentityType { get; private set; } = string.Empty;
    public string PaymentMethod { get; private set; } = string.Empty;
    public Guid? AuthenticatedUserId { get; private set; }
    public Guid? AnonymousId { get; private set; }
    public OrderCustomer Customer { get; private set; } = null!;
    public OrderAddress ShippingAddress { get; private set; } = null!;
    public OrderAddress BillingAddress { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public List<OrderItem> Items { get; private set; } = [];
    public string TrackingCode { get; private set; } = string.Empty;
    public string TransactionId { get; private set; } = string.Empty;
    public string FailureReason { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }

    private Order()
    {
    }

    private Order(
        Guid id,
        Guid cartId,
        Guid userId,
        string identityType,
        string paymentMethod,
        Guid? authenticatedUserId,
        Guid? anonymousId,
        OrderCustomer customer,
        OrderAddress shippingAddress,
        OrderAddress billingAddress,
        IReadOnlyList<OrderItem> items,
        decimal totalAmount,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        CartId = cartId;
        UserId = userId;
        IdentityType = identityType;
        PaymentMethod = paymentMethod;
        AuthenticatedUserId = authenticatedUserId;
        AnonymousId = anonymousId;
        Customer = customer;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        TotalAmount = totalAmount;
        Status = OrderStatus.Pending;
        CreatedAtUtc = createdAtUtc;
        Items = items.ToList();
    }

    public static Order Create(
        Guid cartId,
        Guid userId,
        string identityType,
        string paymentMethod,
        Guid? authenticatedUserId,
        Guid? anonymousId,
        OrderCustomer customer,
        OrderAddress shippingAddress,
        OrderAddress billingAddress,
        IReadOnlyList<OrderItem> items,
        decimal totalAmount)
    {
        if (cartId == Guid.Empty)
        {
            throw new ValidationAppException("Cart id is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new ValidationAppException("User id is required.");
        }

        if (string.IsNullOrWhiteSpace(identityType))
        {
            throw new ValidationAppException("Identity type is required.");
        }

        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            throw new ValidationAppException("Payment method is required.");
        }

        if (items.Count == 0)
        {
            throw new ValidationAppException("Order must contain at least one item.");
        }

        if (totalAmount <= 0)
        {
            throw new ValidationAppException("Order total amount must be greater than zero.");
        }

        var calculatedTotal = items.Sum(x => x.Quantity * x.UnitPrice);
        if (calculatedTotal != totalAmount)
        {
            throw new ValidationAppException("Order total amount does not match the sum of items.");
        }

        return new Order(
            Guid.NewGuid(),
            cartId,
            userId,
            identityType.Trim(),
            paymentMethod.Trim(),
            authenticatedUserId,
            anonymousId,
            customer,
            shippingAddress,
            billingAddress,
            items,
            totalAmount,
            DateTimeOffset.UtcNow);
    }

    public void MarkCompleted(string trackingCode, string transactionId)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new ConflictAppException($"Order '{Id}' cannot be completed from status '{Status}'.");
        }

        Status = OrderStatus.Completed;
        TrackingCode = trackingCode;
        TransactionId = transactionId;
        FailureReason = string.Empty;
    }

    public void MarkCancelled(string reason)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new ConflictAppException($"Order '{Id}' cannot be cancelled from status '{Status}'.");
        }

        Status = OrderStatus.Cancelled;
        FailureReason = reason;
    }
}
