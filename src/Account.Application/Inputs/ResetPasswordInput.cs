namespace Account.Application.Inputs;

public sealed record ResetPasswordInput(string Email, string Code, string NewPassword);
