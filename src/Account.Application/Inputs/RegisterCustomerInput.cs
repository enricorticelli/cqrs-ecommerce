namespace Account.Application.Inputs;

public sealed record RegisterCustomerInput(string Email, string Password, string FirstName, string LastName, string Phone);
