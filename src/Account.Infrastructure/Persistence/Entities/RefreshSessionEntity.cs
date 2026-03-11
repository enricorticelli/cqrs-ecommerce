namespace Account.Infrastructure.Persistence.Entities;

public sealed class RefreshSessionEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Realm { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public DateTimeOffset? RevokedAtUtc { get; set; }
}
