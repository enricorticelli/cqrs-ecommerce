using Catalog.Domain;
using FluentValidation;

namespace Catalog.Application;

public sealed record CreateProductCommand(string Sku, string Name, string Description, decimal Price);

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(1024);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

public sealed record UpdateProductCommand(string Sku, string Name, string Description, decimal Price);

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(1024);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

public interface ICatalogService
{
    Task<IReadOnlyList<ProductDocument>> GetProductsAsync(CancellationToken cancellationToken);
    Task<ProductDocument?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductDocument> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken);
    Task<ProductDocument?> UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken);
}
