using Payment.Application.Abstractions;
using Payment.Application.Models;
using Payment.Application.Queries;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Handlers;

public sealed class GetPaymentSessionsQueryHandler(IPaymentSessionService paymentSessionService)
    : IQueryHandler<GetPaymentSessionsQuery, IReadOnlyList<PaymentSessionView>>
{
    public Task<IReadOnlyList<PaymentSessionView>> HandleAsync(GetPaymentSessionsQuery query, CancellationToken cancellationToken)
    {
        return paymentSessionService.GetAllAsync(query.Limit, cancellationToken);
    }
}
