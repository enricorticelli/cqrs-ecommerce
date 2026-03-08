using Order.Application.Commands;
using Order.Application.Views;

namespace Order.Application.Abstractions.Commands;

public interface IOrderCommandService
{
    Task<OrderView> CreateAsync(CreateOrderCommand command, CancellationToken cancellationToken);
    Task<OrderView> ManualCompleteAsync(ManualCompleteOrderCommand command, CancellationToken cancellationToken);
    Task<OrderView> ManualCancelAsync(ManualCancelOrderCommand command, CancellationToken cancellationToken);
}
