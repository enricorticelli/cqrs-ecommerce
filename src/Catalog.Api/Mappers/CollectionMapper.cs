using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Application.Collections;
using Catalog.Application.Views;

namespace Catalog.Api.Mappers;

public static class CollectionMapper
{
    public static CreateCollectionCommand ToCreateCollectionCommand(CreateCollectionRequest request)
        => new(request.Name, request.Slug, request.Description, request.IsFeatured);

    public static UpdateCollectionCommand ToUpdateCollectionCommand(UpdateCollectionRequest request)
        => new(request.Name, request.Slug, request.Description, request.IsFeatured);

    public static CollectionResponse ToResponse(CollectionView view)
        => new(view.Id, view.Name, view.Slug, view.Description, view.IsFeatured);
}
