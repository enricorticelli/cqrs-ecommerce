namespace Account.Api.Contracts.Responses;

public sealed record VerificationCodeIssuedResponse(bool Issued, string? CodePreview);
