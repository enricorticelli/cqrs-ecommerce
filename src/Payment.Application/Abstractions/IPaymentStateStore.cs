namespace Payment.Application.Abstractions;

public interface IPaymentStateStore
{
    Task<Guid> StartSessionAsync(Guid orderId, Guid userId, decimal amount, string paymentMethod, CancellationToken cancellationToken);
    Task<bool> AuthorizeSessionAsync(Guid sessionId, string transactionId, CancellationToken cancellationToken);
    Task<bool> RejectSessionAsync(Guid sessionId, string reason, CancellationToken cancellationToken);
}
