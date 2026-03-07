using Payment.Api.Contracts.Requests;
using Payment.Api.Contracts.Responses;
using Payment.Application.Commands;
using Payment.Application.Models;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Payment.Api.Mappers;

public static class PaymentMapper
{
    public static AuthorizePaymentCommand ToAuthorizePaymentCommand(AuthorizePaymentRequest request)
    {
        var payload = new PaymentAuthorizeRequestedV1(
            request.OrderId,
            request.UserId,
            request.Amount,
            request.PaymentMethod);

        return new AuthorizePaymentCommand(payload);
    }

    public static PaymentAuthorizeResponse ToPaymentAuthorizeResponse(Guid orderId, bool authorized, string? transactionId)
        => new(orderId, authorized, transactionId);

    public static RejectPaymentSessionCommand ToRejectPaymentSessionCommand(Guid sessionId, RejectPaymentSessionRequest request)
        => new(sessionId, request.Reason ?? string.Empty);

    public static PaymentSessionStatusResponse ToPaymentSessionStatusResponse(Guid sessionId, string status)
        => new(sessionId, status);

    public static PaymentSessionResponse ToResponse(PaymentSessionView view)
        => new(
            view.SessionId,
            view.OrderId,
            view.UserId,
            view.Amount,
            view.PaymentMethod,
            view.Status,
            view.TransactionId,
            view.FailureReason,
            view.CreatedAtUtc,
            view.CompletedAtUtc,
            view.RedirectUrl);
}
