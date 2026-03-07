using Payment.Api.Contracts;
using Payment.Api.Contracts.Requests;
using Payment.Api.Contracts.Responses;
using Shared.BuildingBlocks.Api;

namespace Payment.Api.Endpoints;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(PaymentRoutes.Base)
            .WithTags("Payment");

        group.MapPost("/authorize", AuthorizePayment)
            .WithName("AuthorizePayment");
        group.MapGet("/sessions", ListPaymentSessions)
            .WithName("ListPaymentSessions");
        group.MapGet("/sessions/orders/{orderId:guid}", GetPaymentSessionByOrderId)
            .WithName("GetPaymentSessionByOrderId");
        group.MapGet("/sessions/{sessionId:guid}", GetPaymentSessionById)
            .WithName("GetPaymentSessionById");
        group.MapGet("/hosted/{paymentMethod}", RenderHostedPaymentPage)
            .WithName("RenderHostedPaymentPage");
        group.MapPost("/sessions/{sessionId:guid}/authorize", AuthorizePaymentSession)
            .WithName("AuthorizePaymentSession");
        group.MapPost("/sessions/{sessionId:guid}/reject", RejectPaymentSession)
            .WithName("RejectPaymentSession");
        return group;
    }

    private static IResult AuthorizePayment(AuthorizePaymentRequest request)
    {
        return Results.Ok(new PaymentAuthorizeResponse(request.OrderId, true, "TX-STUB"));
    }

    private static IResult ListPaymentSessions()
    {
        return Results.Ok(new[] { BuildSession(Guid.NewGuid(), Guid.NewGuid()) });
    }

    private static IResult GetPaymentSessionByOrderId(Guid orderId)
    {
        return Results.Ok(BuildSession(Guid.NewGuid(), orderId));
    }

    private static IResult GetPaymentSessionById(Guid sessionId)
    {
        return Results.Ok(BuildSession(sessionId, Guid.NewGuid()));
    }

    private static IResult RenderHostedPaymentPage(string paymentMethod)
    {
        return Results.Content($"Stub hosted page for payment method '{paymentMethod}'.", "text/plain");
    }

    private static IResult AuthorizePaymentSession(Guid sessionId)
    {
        return Results.Ok(new PaymentSessionStatusResponse(sessionId, "Authorized"));
    }

    private static IResult RejectPaymentSession(Guid sessionId, RejectPaymentSessionRequest request)
    {
        _ = request;
        return Results.Ok(new PaymentSessionStatusResponse(sessionId, "Rejected"));
    }

    private static PaymentSessionResponse BuildSession(Guid sessionId, Guid orderId)
    {
        return new PaymentSessionResponse(
            sessionId,
            orderId,
            Guid.NewGuid(),
            10m,
            "card",
            "Pending",
            null,
            null,
            DateTimeOffset.UtcNow,
            null,
            $"http://localhost:8080/v1/payments/hosted/card?sessionId={sessionId}");
    }
}
