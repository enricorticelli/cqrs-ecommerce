using Payment.Application.Models;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Queries;

public sealed record GetPaymentSessionByOrderIdQuery(Guid OrderId) : IQuery<PaymentSessionView?>;
