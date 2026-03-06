using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.Collections;

public sealed record UpdateCollectionCommand(
    [property: Required, StringLength(128)] string Name,
    [property: Required, StringLength(128)] string Slug,
    [property: StringLength(1024)] string Description,
    bool IsFeatured);
