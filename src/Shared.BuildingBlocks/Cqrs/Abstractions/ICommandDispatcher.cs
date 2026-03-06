namespace Shared.BuildingBlocks.Cqrs.Abstractions;

public interface ICommandDispatcher
{
    Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken);
}
