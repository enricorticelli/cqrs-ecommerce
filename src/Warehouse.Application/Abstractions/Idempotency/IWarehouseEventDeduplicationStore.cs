namespace Warehouse.Application.Abstractions.Idempotency;

public interface IWarehouseEventDeduplicationStore
{
    Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken);
    Task MarkProcessedAsync(Guid eventId, CancellationToken cancellationToken);
}
