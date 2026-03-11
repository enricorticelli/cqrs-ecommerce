namespace Account.Application.Models;

public sealed record AccountUserModel(
    Guid Id,
    string Email,
    bool IsEmailVerified,
    string? FirstName,
    string? LastName,
    string? Phone);
