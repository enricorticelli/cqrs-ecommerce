namespace Cart.Domain.Events;

public sealed record CartCreated(Guid CartId, Guid UserId);
