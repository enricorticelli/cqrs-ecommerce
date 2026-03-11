namespace Account.Api.Contracts.Responses;

public sealed record ProfileResponse(Guid UserId, string Email, bool IsEmailVerified, string FirstName, string LastName, string Phone);
