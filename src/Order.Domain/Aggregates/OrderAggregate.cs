using Order.Domain.Enums;
using Order.Domain.Events;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Domain.Aggregates;

public sealed class OrderAggregate
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid UserId { get; private set; }
    public string IdentityType { get; private set; } = OrderIdentityTypes.Anonymous;
    public Guid? AuthenticatedUserId { get; private set; }
    public Guid? AnonymousId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public OrderCustomerDetails Customer { get; private set; } = OrderCustomerDetails.Empty;
    public OrderAddress ShippingAddress { get; private set; } = OrderAddress.Empty;
    public OrderAddress BillingAddress { get; private set; } = OrderAddress.Empty;
    public string FailureReason { get; private set; } = string.Empty;
    public string TrackingCode { get; private set; } = string.Empty;
    public string TransactionId { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public List<OrderItemDto> Items { get; } = [];

    public void Apply(OrderPlacedDomain @event)
    {
        Id = @event.OrderId;
        CartId = @event.CartId;
        UserId = @event.UserId;
        IdentityType = string.IsNullOrWhiteSpace(@event.IdentityType)
            ? OrderIdentityTypes.Anonymous
            : @event.IdentityType;
        AuthenticatedUserId = @event.AuthenticatedUserId;
        AnonymousId = @event.AnonymousId;
        Customer = @event.Customer ?? OrderCustomerDetails.Empty;
        ShippingAddress = @event.ShippingAddress ?? OrderAddress.Empty;
        BillingAddress = @event.BillingAddress ?? OrderAddress.Empty;
        TotalAmount = @event.TotalAmount;
        Items.Clear();
        Items.AddRange(@event.Items);
        Status = OrderStatus.Pending;
    }

    public void Apply(OrderStockReservedDomain _)
    {
        if (Status != OrderStatus.Failed)
        {
            Status = OrderStatus.StockReserved;
        }
    }

    public void Apply(OrderPaymentAuthorizedDomain @event)
    {
        if (Status != OrderStatus.Failed)
        {
            Status = OrderStatus.PaymentAuthorized;
            TransactionId = @event.TransactionId;
        }
    }

    public void Apply(OrderCompletedDomain @event)
    {
        if (Status != OrderStatus.Failed)
        {
            Status = OrderStatus.Completed;
            TrackingCode = @event.TrackingCode;
            TransactionId = @event.TransactionId;
        }
    }

    public void Apply(OrderFailedDomain @event)
    {
        Status = OrderStatus.Failed;
        FailureReason = @event.Reason;
    }
}
