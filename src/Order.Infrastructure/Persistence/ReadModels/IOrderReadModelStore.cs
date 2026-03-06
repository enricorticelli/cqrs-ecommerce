namespace Order.Infrastructure.Persistence.ReadModels;

public interface IOrderReadModelStore : Shared.BuildingBlocks.ReadModels.IReadModelStore<Guid, OrderReadModelRow>
{
    Task<IReadOnlyList<OrderReadModelRow>> ListAsync(int limit, int offset, CancellationToken cancellationToken);
}
