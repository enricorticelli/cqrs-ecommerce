using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Collection;

public sealed class GetCollectionByIdQueryHandler(ICollectionQueryService collectionQueryService)
    : IQueryHandler<GetCollectionByIdQuery, CollectionView?>
{
    public Task<CollectionView?> HandleAsync(GetCollectionByIdQuery query, CancellationToken cancellationToken)
    {
        return collectionQueryService.GetCollectionByIdAsync(query.CollectionId, cancellationToken);
    }
}
