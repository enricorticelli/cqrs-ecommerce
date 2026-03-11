namespace Account.Infrastructure.Persistence.Entities;

public sealed class PasswordResetTokenEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CodeHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public DateTimeOffset? UsedAtUtc { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
