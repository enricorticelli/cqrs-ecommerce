using Order.Api.Contracts;
using Order.Api.Contracts.Requests;
using Order.Api.Contracts.Responses;
using Order.Api.Mappers;
using Order.Application.Abstractions.Commands;
using Order.Application.Abstractions.Queries;
using Order.Application.Commands;
using Shared.BuildingBlocks.Api.Correlation;
using Shared.BuildingBlocks.Api.Errors;
using Shared.BuildingBlocks.Api.Pagination;

namespace Order.Api.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var storeGroup = app.MapGroup(OrderRoutes.StoreBase)
            .WithTags("Order");

        storeGroup.MapPost("/", CreateOrder)
            .WithName("StoreCreateOrder");
        storeGroup.MapGet("/", StoreListOrders)
            .WithName("StoreListOrders");
        storeGroup.MapGet("/{orderId:guid}", GetOrder)
            .WithName("StoreGetOrder");
        storeGroup.MapPost("/{orderId:guid}/manual-cancel", StoreManualCancelOrder)
            .WithName("StoreManualCancelOrder");
        storeGroup.MapPost("/claim-guest", ClaimGuestOrders)
            .WithName("StoreClaimGuestOrders");

        var adminGroup = app.MapGroup(OrderRoutes.AdminBase)
            .WithTags("Order");

        adminGroup.MapGet("/", ListOrders)
            .WithName("AdminListOrders");
        adminGroup.MapGet("/{orderId:guid}", GetOrder)
            .WithName("AdminGetOrder");
        adminGroup.MapPost("/{orderId:guid}/manual-complete", AdminManualCompleteOrder)
            .WithName("AdminManualCompleteOrder");
        adminGroup.MapPost("/{orderId:guid}/manual-cancel", AdminManualCancelOrder)
            .WithName("AdminManualCancelOrder");

        return adminGroup;
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderRequest request,
        IOrderCommandService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var order = await service.CreateAsync(request.ToCreateCommand(correlationId), cancellationToken);
            return Results.Created($"{OrderRoutes.StoreBase}/{order.Id}", new OrderCreatedResponse(order.Id, order.Status));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> ListOrders(
        IOrderQueryService service,
        int? limit,
        int? offset,
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var (normalizedLimit, normalizedOffset) = PaginationNormalizer.Normalize(limit, offset);

        var orders = (await service.ListAsync(cancellationToken))
            .Select(x => x.ToResponse());

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var token = searchTerm.Trim();
            orders = orders.Where(x => MatchesSearch(x, token));
        }

        var page = orders
            .Skip(normalizedOffset)
            .Take(normalizedLimit)
            .ToArray();

        return Results.Ok(page);
    }

    private static async Task<IResult> StoreListOrders(
        IOrderQueryService service,
        Guid? authenticatedUserId,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var (normalizedLimit, normalizedOffset) = PaginationNormalizer.Normalize(limit, offset);

        var source = authenticatedUserId.HasValue
            ? await service.ListByAuthenticatedUserIdAsync(authenticatedUserId.Value, cancellationToken)
            : await service.ListAsync(cancellationToken);

        var page = source
            .Skip(normalizedOffset)
            .Take(normalizedLimit)
            .Select(x => x.ToResponse())
            .ToArray();

        return Results.Ok(page);
    }

    private static async Task<IResult> GetOrder(
        Guid orderId,
        IOrderQueryService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await service.GetByIdAsync(orderId, cancellationToken);
            var response = order.ToResponse();

            return Results.Ok(response);
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> AdminManualCompleteOrder(
        Guid orderId,
        ManualCompleteOrderRequest request,
        IOrderCommandService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await service.AdminManualCompleteAsync(new ManualCompleteOrderCommand(orderId, request.TrackingCode, request.TransactionId), cancellationToken);
            return Results.Ok(new ManualCompleteOrderResponse(order.Id, order.Status, order.TrackingCode, order.TransactionId, "manual"));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> AdminManualCancelOrder(
        Guid orderId,
        ManualCancelOrderRequest request,
        IOrderCommandService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await service.AdminManualCancelAsync(new ManualCancelOrderCommand(orderId, request.Reason), cancellationToken);
            return Results.Ok(new ManualCancelOrderResponse(order.Id, order.Status, order.FailureReason, "manual"));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> StoreManualCancelOrder(
        Guid orderId,
        ManualCancelOrderRequest request,
        IOrderCommandService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await service.StoreManualCancelAsync(new ManualCancelOrderCommand(orderId, request.Reason), cancellationToken);
            return Results.Ok(new ManualCancelOrderResponse(order.Id, order.Status, order.FailureReason, "manual"));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> ClaimGuestOrders(
        ClaimGuestOrdersRequest request,
        IOrderCommandService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var claimedCount = await service.ClaimGuestOrdersAsync(
                new ClaimGuestOrdersCommand(request.AuthenticatedUserId, request.CustomerEmail),
                cancellationToken);

            return Results.Ok(new ClaimGuestOrdersResponse(claimedCount));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static bool MatchesSearch(OrderResponse response, string searchToken)
    {
        return response.Id.ToString().Contains(searchToken, StringComparison.OrdinalIgnoreCase)
            || response.CartId.ToString().Contains(searchToken, StringComparison.OrdinalIgnoreCase)
            || response.UserId.ToString().Contains(searchToken, StringComparison.OrdinalIgnoreCase)
            || response.Status.Contains(searchToken, StringComparison.OrdinalIgnoreCase)
            || response.PaymentMethod.Contains(searchToken, StringComparison.OrdinalIgnoreCase)
            || response.IdentityType.Contains(searchToken, StringComparison.OrdinalIgnoreCase)
            || (response.TrackingCode?.Contains(searchToken, StringComparison.OrdinalIgnoreCase) ?? false)
            || (response.TransactionId?.Contains(searchToken, StringComparison.OrdinalIgnoreCase) ?? false)
            || (response.FailureReason?.Contains(searchToken, StringComparison.OrdinalIgnoreCase) ?? false)
            || (response.AuthenticatedUserId?.ToString().Contains(searchToken, StringComparison.OrdinalIgnoreCase) ?? false)
            || (response.AnonymousId?.ToString().Contains(searchToken, StringComparison.OrdinalIgnoreCase) ?? false)
            || response.Customer.FirstName.Contains(searchToken, StringComparison.OrdinalIgnoreCase)
            || response.Customer.LastName.Contains(searchToken, StringComparison.OrdinalIgnoreCase)
            || response.Customer.Email.Contains(searchToken, StringComparison.OrdinalIgnoreCase);
    }
}
