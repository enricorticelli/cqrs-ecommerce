namespace Account.Api.Contracts.Requests;

public sealed record RegisterCustomerRequest(string Email, string Password, string FirstName, string LastName, string Phone);
