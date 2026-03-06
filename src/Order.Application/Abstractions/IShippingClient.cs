using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Application.Abstractions;

public interface IShippingClient
{
    Task<string> CreateShipmentAsync(Guid orderId, Guid userId, IReadOnlyList<OrderItemDto> items, CancellationToken cancellationToken);
}
