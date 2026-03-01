using Catalog.Application;
using Catalog.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Http;

namespace Catalog.Api.Endpoints;

public static partial class CatalogEndpoints
{
    private static void MapBrandEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", GetBrands).WithName("GetBrands");
        group.MapGet("/{id:guid}", GetBrandById).WithName("GetBrandById");
        group.MapPost("/", CreateBrand).WithName("CreateBrand");
        group.MapPut("/{id:guid}", UpdateBrand).WithName("UpdateBrand");
        group.MapDelete("/{id:guid}", DeleteBrand).WithName("DeleteBrand");
    }

    private static async Task<Ok<IReadOnlyList<BrandDocument>>> GetBrands(ICatalogService service, CancellationToken cancellationToken)
    {
        var brands = await service.GetBrandsAsync(cancellationToken);
        return TypedResults.Ok(brands);
    }

    private static async Task<Results<Ok<BrandDocument>, NotFound>> GetBrandById(Guid id, ICatalogService service, CancellationToken cancellationToken)
    {
        var brand = await service.GetBrandByIdAsync(id, cancellationToken);
        return brand is null ? TypedResults.NotFound() : TypedResults.Ok(brand);
    }

    private static async Task<Results<Created<BrandDocument>, ProblemHttpResult>> CreateBrand(
        CreateBrandCommand command,
        ICatalogService service,
        CancellationToken cancellationToken)
    {
        var errors = command.GetValidationErrors();
        if (errors.Count != 0)
        {
            return CreateValidationProblem(errors, "Invalid brand payload");
        }

        var brand = await service.CreateBrandAsync(command, cancellationToken);
        return TypedResults.Created($"/v1/brands/{brand.Id}", brand);
    }

    private static async Task<Results<Ok<BrandDocument>, NotFound, ProblemHttpResult>> UpdateBrand(
        Guid id,
        UpdateBrandCommand command,
        ICatalogService service,
        CancellationToken cancellationToken)
    {
        var errors = command.GetValidationErrors();
        if (errors.Count != 0)
        {
            return CreateValidationProblem(errors, "Invalid brand payload");
        }

        var brand = await service.UpdateBrandAsync(id, command, cancellationToken);
        return brand is null ? TypedResults.NotFound() : TypedResults.Ok(brand);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteBrand(Guid id, ICatalogService service, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteBrandAsync(id, cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
