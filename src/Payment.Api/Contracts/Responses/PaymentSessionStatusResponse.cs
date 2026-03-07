namespace Payment.Api.Contracts.Responses;

public sealed record PaymentSessionStatusResponse(Guid SessionId, string Status);
