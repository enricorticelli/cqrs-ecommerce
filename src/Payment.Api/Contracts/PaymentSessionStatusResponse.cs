namespace Payment.Api.Contracts;

public sealed record PaymentSessionStatusResponse(Guid SessionId, string Status);
