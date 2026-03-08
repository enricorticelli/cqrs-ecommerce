using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Shipping.Infrastructure.Persistence;

#nullable disable

namespace Shipping.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ShippingDbContext))]
    [Migration("20260308170000_AddShipmentNotificationSnapshot")]
    public partial class AddShipmentNotificationSnapshot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "shipment_notification_snapshots",
                schema: "shipping",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipment_notification_snapshots", x => x.OrderId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shipment_notification_snapshots",
                schema: "shipping");
        }
    }
}
