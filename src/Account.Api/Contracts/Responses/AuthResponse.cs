namespace Account.Api.Contracts.Responses;

public sealed record AuthResponse(
    string AccessToken,
    string AccessTokenExpiresAtUtc,
    string RefreshToken,
    string RefreshTokenExpiresAtUtc,
    string Realm,
    Guid UserId,
    string Email,
    string[] Permissions);
