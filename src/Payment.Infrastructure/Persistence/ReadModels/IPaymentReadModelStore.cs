namespace Payment.Infrastructure.Persistence.ReadModels;

public interface IPaymentReadModelStore
{
    Task<PaymentReadModelRow?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);
    Task<PaymentReadModelRow?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentReadModelRow>> ListAsync(int limit, CancellationToken cancellationToken);
    Task UpsertAsync(PaymentReadModelRow model, CancellationToken cancellationToken);
}
