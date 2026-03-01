using Catalog.Application;
using Catalog.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Catalog.Api.Endpoints;

public static class CatalogEndpoints
{
    public static RouteGroupBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/products")
            .WithTags("Catalog");

        group.MapGet("/", GetProducts)
            .WithName("GetProducts");

        group.MapGet("/{id:guid}", GetProductById)
            .WithName("GetProductById");

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct");

        group.MapPut("/{id:guid}", UpdateProduct)
            .WithName("UpdateProduct");

        group.MapDelete("/{id:guid}", DeleteProduct)
            .WithName("DeleteProduct");

        return group;
    }

    private static async Task<Ok<IReadOnlyList<ProductDocument>>> GetProducts(ICatalogService service, CancellationToken cancellationToken)
    {
        var products = await service.GetProductsAsync(cancellationToken);
        return TypedResults.Ok(products);
    }

    private static async Task<Results<Ok<ProductDocument>, NotFound>> GetProductById(Guid id, ICatalogService service, CancellationToken cancellationToken)
    {
        var product = await service.GetProductByIdAsync(id, cancellationToken);
        return product is null ? TypedResults.NotFound() : TypedResults.Ok(product);
    }

    private static async Task<Results<Created<ProductDocument>, ProblemHttpResult>> CreateProduct(
        CreateProductCommand command,
        IValidator<CreateProductCommand> validator,
        ICatalogService service,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return CreateValidationProblem(validation);
        }

        var product = await service.CreateProductAsync(command, cancellationToken);
        return TypedResults.Created($"/v1/products/{product.Id}", product);
    }

    private static async Task<Results<Ok<ProductDocument>, NotFound, ProblemHttpResult>> UpdateProduct(
        Guid id,
        UpdateProductCommand command,
        IValidator<UpdateProductCommand> validator,
        ICatalogService service,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return CreateValidationProblem(validation);
        }

        var product = await service.UpdateProductAsync(id, command, cancellationToken);
        return product is null ? TypedResults.NotFound() : TypedResults.Ok(product);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteProduct(Guid id, ICatalogService service, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteProductAsync(id, cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static ProblemHttpResult CreateValidationProblem(FluentValidation.Results.ValidationResult validation)
    {
        var errors = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

        return TypedResults.Problem(
            title: "Validation error",
            detail: "Invalid product payload",
            statusCode: StatusCodes.Status400BadRequest,
            extensions: new Dictionary<string, object?> { ["errors"] = errors });
    }
}
