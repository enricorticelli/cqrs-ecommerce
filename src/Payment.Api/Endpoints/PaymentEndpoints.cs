using Payment.Api.Contracts;
using Payment.Api.Contracts.Requests;
using Payment.Api.Contracts.Responses;
using Payment.Api.Mappers;
using Payment.Application.Abstractions.Commands;
using Payment.Application.Abstractions.Queries;
using Shared.BuildingBlocks.Api.Correlation;

namespace Payment.Api.Endpoints;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(PaymentRoutes.StoreBase)
            .WithTags("Payment");

        group.MapGet("/sessions/orders/{orderId:guid}", GetPaymentSessionByOrderId)
            .WithName("StoreGetPaymentSessionByOrderId");
        group.MapGet("/sessions/{sessionId:guid}", GetPaymentSessionById)
            .WithName("StoreGetPaymentSessionById");
        group.MapPost("/sessions/{sessionId:guid}/authorize", AuthorizePaymentSession)
            .WithName("StoreAuthorizePaymentSession");
        group.MapPost("/sessions/{sessionId:guid}/reject", RejectPaymentSession)
            .WithName("StoreRejectPaymentSession");
        return group;
    }

    private static async Task<IResult> GetPaymentSessionByOrderId(
        Guid orderId,
        IConfiguration configuration,
        IPaymentQueryService queryService,
        CancellationToken cancellationToken)
    {
        var redirectUrl = BuildHostedRedirectUrlTemplate(configuration, orderId);
        var session = await queryService.GetOrCreateByOrderIdAsync(orderId, redirectUrl, cancellationToken);

        return Results.Ok(PaymentMapper.ToResponse(session));
    }

    private static async Task<IResult> GetPaymentSessionById(
        Guid sessionId,
        IPaymentQueryService queryService,
        CancellationToken cancellationToken)
    {
        var session = await queryService.GetBySessionIdAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(PaymentMapper.ToResponse(session));
    }

    private static async Task<IResult> AuthorizePaymentSession(
        Guid sessionId,
        IPaymentCommandService commandService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var correlationId = CorrelationIdResolver.Resolve(httpContext);
        var update = await commandService.AuthorizeAsync(sessionId, correlationId, cancellationToken);
        if (update is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new PaymentSessionStatusResponse(sessionId, update.Session.Status));
    }

    private static async Task<IResult> RejectPaymentSession(
        Guid sessionId,
        RejectPaymentSessionRequest request,
        IPaymentCommandService commandService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var correlationId = CorrelationIdResolver.Resolve(httpContext);
        var update = await commandService.RejectAsync(sessionId, request.Reason, correlationId, cancellationToken);
        if (update is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new PaymentSessionStatusResponse(sessionId, update.Session.Status));
    }

    private static string BuildHostedRedirectUrlTemplate(IConfiguration configuration, Guid orderId)
    {
        var baseUrl = configuration["Payment:HostedReturnBaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = "http://localhost:3000";
        }

        return $"{baseUrl.TrimEnd('/')}/payment/session/{{sessionId}}?orderId={orderId}";
    }
}
