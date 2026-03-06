using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Shared.BuildingBlocks.Cqrs.Pipeline;

public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next);
}
