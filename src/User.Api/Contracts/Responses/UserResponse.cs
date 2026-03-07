namespace User.Api.Contracts.Responses;

public sealed record UserResponse(Guid Id, string Email, string FullName);
