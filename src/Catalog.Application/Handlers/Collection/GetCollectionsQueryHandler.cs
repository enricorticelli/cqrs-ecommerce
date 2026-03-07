using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Collection;

public sealed class GetCollectionsQueryHandler(ICollectionQueryService collectionQueryService)
    : IQueryHandler<GetCollectionsQuery, IReadOnlyList<CollectionView>>
{
    public Task<IReadOnlyList<CollectionView>> HandleAsync(GetCollectionsQuery query, CancellationToken cancellationToken)
    {
        return collectionQueryService.GetCollectionsAsync(query.Limit, query.Offset, cancellationToken);
    }
}
