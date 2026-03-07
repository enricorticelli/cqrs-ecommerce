using Payment.Application.Models;

namespace Payment.Application.Abstractions;

public interface IPaymentReadStore
{
    Task<PaymentSessionView?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<PaymentSessionView?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentSessionView>> GetAllAsync(int limit, CancellationToken cancellationToken);
}
