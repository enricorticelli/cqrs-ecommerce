namespace Order.Application;

public interface IPaymentClient
{
    Task<PaymentAuthorizationDecision> AuthorizeAsync(Guid orderId, Guid userId, decimal amount, CancellationToken cancellationToken);
}
