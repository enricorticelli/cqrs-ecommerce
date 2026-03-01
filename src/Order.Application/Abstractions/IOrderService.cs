namespace Order.Application;

public interface IOrderService
{
    Task<OrderCreationResult?> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken);
    Task<OrderView?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken);
}
