namespace Order.Application.Abstractions;

public interface ICartSnapshotClient
{
    Task<CartSnapshot?> GetCartAsync(Guid cartId, CancellationToken cancellationToken);
}
