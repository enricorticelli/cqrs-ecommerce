namespace Account.Api.Contracts.Requests;

public sealed record ResetPasswordRequest(string Email, string Code, string NewPassword);
