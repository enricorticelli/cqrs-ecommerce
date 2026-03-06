namespace Shared.BuildingBlocks.Cqrs.Pipeline;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
