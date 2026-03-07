using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Application.Abstractions.Brands;
using Shared.BuildingBlocks.Api.Correlation;
using Shared.BuildingBlocks.Api.Errors;

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

    private static async Task<IResult> GetBrands(string? searchTerm, IBrandService service, CancellationToken cancellationToken)
    {
        var brands = await service.GetBrandsAsync(searchTerm, cancellationToken);
        return Results.Ok(brands.Select(x => new BrandResponse(x.Id, x.Name, x.Slug, x.Description)));
    }

    private static async Task<IResult> GetBrandById(Guid id, IBrandService service, CancellationToken cancellationToken)
    {
        try
        {
            var brand = await service.GetBrandAsync(id, cancellationToken);
            return Results.Ok(new BrandResponse(brand.Id, brand.Name, brand.Slug, brand.Description));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> CreateBrand(CreateBrandRequest request, IBrandService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var brand = await service.CreateBrandAsync(request.Name, request.Slug, request.Description, correlationId, cancellationToken);
            var response = new BrandResponse(brand.Id, brand.Name, brand.Slug, brand.Description);
            return Results.Created($"{CatalogRoutes.Brands}/{brand.Id}", response);
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> UpdateBrand(Guid id, UpdateBrandRequest request, IBrandService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var brand = await service.UpdateBrandAsync(id, request.Name, request.Slug, request.Description, correlationId, cancellationToken);
            return Results.Ok(new BrandResponse(brand.Id, brand.Name, brand.Slug, brand.Description));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> DeleteBrand(Guid id, IBrandService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            await service.DeleteBrandAsync(id, correlationId, cancellationToken);
            return Results.NoContent();
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }
}
