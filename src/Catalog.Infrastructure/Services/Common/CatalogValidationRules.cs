using Shared.BuildingBlocks.Exceptions;

namespace Catalog.Infrastructure.Services.Common;

public static class CatalogValidationRules
{
    public static void EnsurePriceIsPositive(decimal price)
    {
        if (price <= 0)
        {
            throw new ValidationAppException("Product price must be greater than zero.");
        }
    }
}
