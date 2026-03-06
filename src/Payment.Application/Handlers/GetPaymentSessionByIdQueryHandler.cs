using Payment.Application.Abstractions;
using Payment.Application.Models;
using Payment.Application.Queries;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Handlers;

public sealed class GetPaymentSessionByIdQueryHandler(IPaymentSessionService paymentSessionService)
    : IQueryHandler<GetPaymentSessionByIdQuery, PaymentSessionView?>
{
    public Task<PaymentSessionView?> HandleAsync(GetPaymentSessionByIdQuery query, CancellationToken cancellationToken)
    {
        return paymentSessionService.GetBySessionIdAsync(query.SessionId, cancellationToken);
    }
}
