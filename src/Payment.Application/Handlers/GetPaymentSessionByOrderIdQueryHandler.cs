using Payment.Application.Abstractions;
using Payment.Application.Models;
using Payment.Application.Queries;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Handlers;

public sealed class GetPaymentSessionByOrderIdQueryHandler(IPaymentSessionService paymentSessionService)
    : IQueryHandler<GetPaymentSessionByOrderIdQuery, PaymentSessionView?>
{
    public Task<PaymentSessionView?> HandleAsync(GetPaymentSessionByOrderIdQuery query, CancellationToken cancellationToken)
    {
        return paymentSessionService.GetByOrderIdAsync(query.OrderId, cancellationToken);
    }
}
