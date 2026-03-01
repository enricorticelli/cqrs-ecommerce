using Shared.BuildingBlocks.Contracts;

namespace Order.Application;

public interface IShippingClient
{
    Task<string> CreateShipmentAsync(Guid orderId, Guid userId, IReadOnlyList<OrderItemDto> items, CancellationToken cancellationToken);
}
