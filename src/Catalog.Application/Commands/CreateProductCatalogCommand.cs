using Catalog.Application.Products;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Commands;

public sealed record CreateProductCatalogCommand(CreateProductCommand Product) : ICommand<ProductView?>;
