namespace Account.Application.Inputs;

public sealed record VerifyEmailInput(string Email, string Code);
