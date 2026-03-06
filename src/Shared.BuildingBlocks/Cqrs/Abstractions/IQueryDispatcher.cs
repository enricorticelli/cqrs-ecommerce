namespace Shared.BuildingBlocks.Cqrs.Abstractions;

public interface IQueryDispatcher
{
    Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken);
}
