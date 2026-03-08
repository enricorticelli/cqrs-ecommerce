using Payment.Application.Views;

namespace Payment.Application.Abstractions.Queries;

public interface IPaymentQueryService
{
    Task<IReadOnlyList<PaymentSessionView>> ListAsync(CancellationToken cancellationToken);
    Task<PaymentSessionView> GetOrCreateByOrderIdAsync(Guid orderId, string redirectUrl, CancellationToken cancellationToken);
    Task<PaymentSessionView?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);
}
