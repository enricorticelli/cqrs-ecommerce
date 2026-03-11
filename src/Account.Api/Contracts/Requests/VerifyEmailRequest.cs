namespace Account.Api.Contracts.Requests;

public sealed record VerifyEmailRequest(string Email, string Code);
