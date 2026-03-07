namespace Payment.Api.Contracts.Requests;

public sealed record AuthorizePaymentRequest(Guid OrderId, Guid UserId, decimal Amount, string PaymentMethod);
