using Catalog.Infrastructure.Persistence;
using Shared.BuildingBlocks.Contracts.IntegrationEvents;
using Wolverine.EntityFrameworkCore;

namespace Catalog.Infrastructure.Services.Common;

public abstract class CatalogServiceBase(
    CatalogDbContext dbContext,
    IDbContextOutbox<CatalogDbContext> outbox)
{
    protected CatalogDbContext DbContext { get; } = dbContext;

    protected static IntegrationEventMetadata CreateMetadata(string correlationId)
    {
        return new IntegrationEventMetadata(Guid.NewGuid(), DateTimeOffset.UtcNow, correlationId, "Catalog");
    }

    protected async Task PublishAndFlushAsync(IntegrationEventBase message, CancellationToken cancellationToken)
    {
        await outbox.PublishAsync(message);
        await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }

    protected static IReadOnlyList<Guid> DistinctIds(IReadOnlyList<Guid> ids)
    {
        return ids.Distinct().ToArray();
    }
}
