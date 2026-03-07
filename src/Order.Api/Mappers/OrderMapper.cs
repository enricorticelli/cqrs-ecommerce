using Order.Api.Contracts.Requests;
using Order.Api.Contracts.Responses;
using Order.Application;
using Order.Application.Commands;
using Order.Application.Models;
using Order.Application.Views;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Api.Mappers;

public static class OrderMapper
{
    public static CreateOrderCommand ToCreateOrderCommand(CreateOrderRequest request)
        => new(
            request.CartId,
            request.UserId,
            request.IdentityType,
            request.PaymentMethod,
            request.Items.Select(item => new OrderItemDto(item.ProductId, item.Sku, item.Name, item.Quantity, item.UnitPrice)).ToArray(),
            request.TotalAmount,
            request.AuthenticatedUserId,
            request.AnonymousId,
            request.Customer is null
                ? null
                : new OrderCustomerDetails(request.Customer.FirstName, request.Customer.LastName, request.Customer.Email, request.Customer.Phone),
            request.ShippingAddress is null
                ? null
                : new OrderAddress(request.ShippingAddress.Street, request.ShippingAddress.City, request.ShippingAddress.PostalCode, request.ShippingAddress.Country),
            request.BillingAddress is null
                ? null
                : new OrderAddress(request.BillingAddress.Street, request.BillingAddress.City, request.BillingAddress.PostalCode, request.BillingAddress.Country));

    public static OrderCreatedResponse ToOrderCreatedResponse(Guid orderId, string status)
        => new(orderId, status);

    public static ManualCompleteOrderCommand ToManualCompleteOrderCommand(Guid orderId, ManualCompleteOrderRequest request)
        => new(orderId, request.TrackingCode, request.TransactionId);

    public static ManualCompleteOrderResponse ToManualCompleteOrderResponse(Guid orderId, ManualOrderActionResult result)
        => new(
            orderId,
            result.Status,
            result.TrackingCode ?? string.Empty,
            result.TransactionId ?? string.Empty,
            "manual");

    public static ManualCancelOrderCommand ToManualCancelOrderCommand(Guid orderId, ManualCancelOrderRequest request)
        => new(orderId, request.Reason);

    public static ManualCancelOrderResponse ToManualCancelOrderResponse(Guid orderId, ManualOrderActionResult result)
        => new(
            orderId,
            result.Status,
            result.Reason ?? string.Empty,
            "manual");

    public static OrderResponse ToResponse(OrderView view)
        => new(
            view.Id,
            view.CartId,
            view.UserId,
            view.IdentityType,
            view.PaymentMethod,
            view.AuthenticatedUserId,
            view.AnonymousId,
            new OrderCustomerResponse(view.Customer.FirstName, view.Customer.LastName, view.Customer.Email, view.Customer.Phone),
            new OrderAddressResponse(view.ShippingAddress.Street, view.ShippingAddress.City, view.ShippingAddress.PostalCode, view.ShippingAddress.Country),
            new OrderAddressResponse(view.BillingAddress.Street, view.BillingAddress.City, view.BillingAddress.PostalCode, view.BillingAddress.Country),
            view.Status,
            view.TotalAmount,
            view.Items.Select(item => new OrderItemResponse(item.ProductId, item.Sku, item.Name, item.Quantity, item.UnitPrice)).ToArray(),
            view.TrackingCode,
            view.TransactionId,
            view.FailureReason);
}
