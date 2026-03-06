namespace Shared.BuildingBlocks.Cqrs.Abstractions;

public interface IQuery<TResult> : IRequest<TResult>;
