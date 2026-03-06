namespace Shared.BuildingBlocks.Cqrs.Abstractions;

public interface ICommand<TResult> : IRequest<TResult>;
