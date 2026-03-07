using Catalog.Application.Commands;

namespace Catalog.Application.Products;

public sealed record CreateProductCatalogCommand(CreateProductCommand Payload);
