using Order.Application.Views;

namespace Order.Application.Abstractions;

public interface IOrderCommandService
{
    Task<OrderCreationResult?> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken);
}
