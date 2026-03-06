using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.Products;

public sealed record CreateProductCommand(
    [property: Required, StringLength(64)] string Sku,
    [property: Required, StringLength(256)] string Name,
    [property: StringLength(1024)] string Description,
    [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335")] decimal Price,
    Guid BrandId,
    Guid CategoryId,
    [property: Required] IReadOnlyList<Guid> CollectionIds,
    bool IsNewArrival,
    bool IsBestSeller) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BrandId == Guid.Empty)
        {
            yield return new ValidationResult("The BrandId field must be a non-empty GUID.", [nameof(BrandId)]);
        }

        if (CategoryId == Guid.Empty)
        {
            yield return new ValidationResult("The CategoryId field must be a non-empty GUID.", [nameof(CategoryId)]);
        }
    }
}
