using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.Categories;

public sealed record UpdateCategoryCommand(
    [property: Required, StringLength(128)] string Name,
    [property: Required, StringLength(128)] string Slug,
    [property: StringLength(1024)] string Description);
