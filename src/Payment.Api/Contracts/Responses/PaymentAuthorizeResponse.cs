namespace Payment.Api.Contracts.Responses;

public sealed record PaymentAuthorizeResponse(Guid OrderId, bool Authorized, string? TransactionId);
