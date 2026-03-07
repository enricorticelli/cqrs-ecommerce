using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Application.Categories;
using Catalog.Application.Views;

namespace Catalog.Api.Mappers;

public static class CategoryMapper
{
    public static CreateCategoryCommand ToCreateCategoryCommand(CreateCategoryRequest request)
        => new(request.Name, request.Slug, request.Description);

    public static UpdateCategoryCommand ToUpdateCategoryCommand(UpdateCategoryRequest request)
        => new(request.Name, request.Slug, request.Description);

    public static CategoryResponse ToResponse(CategoryView view)
        => new(view.Id, view.Name, view.Slug, view.Description);
}
