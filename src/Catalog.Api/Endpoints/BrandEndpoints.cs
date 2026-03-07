using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Api.Mappers;
using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Api.Endpoints;

public static class BrandEndpoints
{
    public static RouteGroupBuilder MapBrandEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CatalogRoutes.Brands)
            .WithTags("Catalog")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

        group.MapGet("/", GetBrands).WithName("GetBrands");
        group.MapGet("/{id:guid}", GetBrandById).WithName("GetBrandById");
        group.MapPost("/", CreateBrand).WithName("CreateBrand");
        group.MapPut("/{id:guid}", UpdateBrand).WithName("UpdateBrand");
        group.MapDelete("/{id:guid}", DeleteBrand).WithName("DeleteBrand");

        return group;
    }

    private static async Task<Ok<IReadOnlyList<BrandResponse>>> GetBrands(
        IQueryDispatcher queryDispatcher,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit ?? 200, 1, 200);
        var safeOffset = Math.Max(offset ?? 0, 0);
        var brands = await queryDispatcher.ExecuteAsync(new GetBrandsQuery(safeLimit, safeOffset), cancellationToken);
        IReadOnlyList<BrandResponse> response = brands.Select(BrandMapper.ToResponse).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<BrandResponse>, NotFound>> GetBrandById(Guid id, IQueryDispatcher queryDispatcher, CancellationToken cancellationToken)
    {
        var brand = await queryDispatcher.ExecuteAsync(new GetBrandByIdQuery(id), cancellationToken);
        return brand is null ? TypedResults.NotFound() : TypedResults.Ok(BrandMapper.ToResponse(brand));
    }

    private static async Task<Created<BrandResponse>> CreateBrand(
        CreateBrandRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = BrandMapper.ToCreateBrandCommand(request);
        var brand = await commandDispatcher.ExecuteAsync(new CreateBrandCatalogCommand(command), cancellationToken);
        return TypedResults.Created($"/v1/brands/{brand.Id}", BrandMapper.ToResponse(brand));
    }

    private static async Task<Results<Ok<BrandResponse>, NotFound>> UpdateBrand(
        Guid id,
        UpdateBrandRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = BrandMapper.ToUpdateBrandCommand(request);
        var brand = await commandDispatcher.ExecuteAsync(new UpdateBrandCatalogCommand(id, command), cancellationToken);
        return brand is null ? TypedResults.NotFound() : TypedResults.Ok(BrandMapper.ToResponse(brand));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteBrand(Guid id, ICommandDispatcher commandDispatcher, CancellationToken cancellationToken)
    {
        var deleted = await commandDispatcher.ExecuteAsync(new DeleteBrandCatalogCommand(id), cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
