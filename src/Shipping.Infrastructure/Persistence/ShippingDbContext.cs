using Microsoft.EntityFrameworkCore;
using Shipping.Infrastructure.Persistence.Entities;

namespace Shipping.Infrastructure.Persistence;

public sealed class ShippingDbContext(DbContextOptions<ShippingDbContext> options) : DbContext(options)
{
    public DbSet<Shipping.Domain.Entities.Shipment> Shipments => Set<Shipping.Domain.Entities.Shipment>();
    public DbSet<ShipmentNotificationSnapshot> ShipmentNotificationSnapshots => Set<ShipmentNotificationSnapshot>();
    public DbSet<ProcessedShippingIntegrationEvent> ProcessedIntegrationEvents => Set<ProcessedShippingIntegrationEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("shipping");

        modelBuilder.Entity<Shipping.Domain.Entities.Shipment>(entity =>
        {
            entity.ToTable("shipments");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.OrderId).IsRequired();
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.TrackingCode).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.Property(x => x.DeliveredAtUtc).IsRequired(false);

            entity.HasIndex(x => x.OrderId).IsUnique();
            entity.HasIndex(x => x.TrackingCode).IsUnique();
        });

        modelBuilder.Entity<ProcessedShippingIntegrationEvent>(entity =>
        {
            entity.ToTable("processed_integration_events");
            entity.HasKey(x => x.EventId);
            entity.Property(x => x.ProcessedAtUtc).IsRequired();
        });

        modelBuilder.Entity<ShipmentNotificationSnapshot>(entity =>
        {
            entity.ToTable("shipment_notification_snapshots");
            entity.HasKey(x => x.OrderId);
            entity.Property(x => x.CustomerEmail).HasMaxLength(256).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
