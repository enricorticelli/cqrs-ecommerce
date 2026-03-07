using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;

namespace Catalog.Api.Endpoints;

public static class BrandEndpoints
{
    public static RouteGroupBuilder MapBrandEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CatalogRoutes.Brands)
            .WithTags("Catalog");

        group.MapGet("/", GetBrands).WithName("GetBrands");
        group.MapGet("/{id:guid}", GetBrandById).WithName("GetBrandById");
        group.MapPost("/", CreateBrand).WithName("CreateBrand");
        group.MapPut("/{id:guid}", UpdateBrand).WithName("UpdateBrand");
        group.MapDelete("/{id:guid}", DeleteBrand).WithName("DeleteBrand");

        return group;
    }

    private static IResult GetBrands()
    {
        return Results.Ok(new[] { BuildBrandResponse(Guid.NewGuid()) });
    }

    private static IResult GetBrandById(Guid id)
    {
        return Results.Ok(BuildBrandResponse(id));
    }

    private static IResult CreateBrand(CreateBrandRequest request)
    {
        var id = Guid.NewGuid();
        var response = BuildBrandResponse(id, request.Name, request.Slug, request.Description);
        return Results.Created($"{CatalogRoutes.Brands}/{id}", response);
    }

    private static IResult UpdateBrand(Guid id, UpdateBrandRequest request)
    {
        var response = BuildBrandResponse(id, request.Name, request.Slug, request.Description);
        return Results.Ok(response);
    }

    private static IResult DeleteBrand(Guid id)
    {
        _ = id;
        return Results.NoContent();
    }

    private static BrandResponse BuildBrandResponse(Guid id, string name = "Stub brand", string slug = "stub-brand", string? description = "Stub description")
    {
        return new BrandResponse(id, name, slug, description ?? string.Empty);
    }
}
