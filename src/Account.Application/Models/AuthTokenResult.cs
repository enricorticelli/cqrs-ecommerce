namespace Account.Application.Models;

public sealed record AuthTokenResult(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    string Realm,
    Guid UserId,
    string Email,
    string[] Permissions);
