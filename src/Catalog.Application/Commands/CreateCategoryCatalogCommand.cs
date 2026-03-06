using Catalog.Application.Categories;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Commands;

public sealed record CreateCategoryCatalogCommand(CreateCategoryCommand Category) : ICommand<CategoryView>;
