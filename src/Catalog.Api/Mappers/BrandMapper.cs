using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Application.Brands;
using Catalog.Application.Views;

namespace Catalog.Api.Mappers;

public static class BrandMapper
{
    public static CreateBrandCommand ToCreateBrandCommand(CreateBrandRequest request)
        => new(request.Name, request.Slug, request.Description);

    public static UpdateBrandCommand ToUpdateBrandCommand(UpdateBrandRequest request)
        => new(request.Name, request.Slug, request.Description);

    public static BrandResponse ToResponse(BrandView view)
        => new(view.Id, view.Name, view.Slug, view.Description);
}
