namespace Order.Application;

public interface ICartSnapshotClient
{
    Task<CartSnapshot?> GetCartAsync(Guid cartId, CancellationToken cancellationToken);
}
