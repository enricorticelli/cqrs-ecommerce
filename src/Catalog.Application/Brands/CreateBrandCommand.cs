using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.Brands;

public sealed record CreateBrandCommand(
    [property: Required, StringLength(128)] string Name,
    [property: Required, StringLength(128)] string Slug,
    [property: StringLength(1024)] string Description);
