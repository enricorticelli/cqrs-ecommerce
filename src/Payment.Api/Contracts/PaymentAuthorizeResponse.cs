namespace Payment.Api.Contracts;

public sealed record PaymentAuthorizeResponse(Guid OrderId, bool Authorized, string? TransactionId);
