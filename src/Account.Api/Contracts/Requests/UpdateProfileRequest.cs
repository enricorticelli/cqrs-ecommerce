namespace Account.Api.Contracts.Requests;

public sealed record UpdateProfileRequest(string FirstName, string LastName, string Phone);
