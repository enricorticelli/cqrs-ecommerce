using Order.Application.Models;

namespace Order.Application.Abstractions;

public interface IPaymentClient
{
    Task<PaymentAuthorizationDecision> AuthorizeAsync(Guid orderId, Guid userId, decimal amount, CancellationToken cancellationToken);
}
