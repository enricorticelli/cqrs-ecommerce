namespace Shipping.Domain.Enums;

public static class ShipmentStatuses
{
    public const string Preparing = "Preparing";
    public const string Created = "Created";
    public const string InTransit = "InTransit";
    public const string Delivered = "Delivered";
    public const string Cancelled = "Cancelled";

    public static bool IsSupported(string value)
    {
        return value is Preparing or Created or InTransit or Delivered or Cancelled;
    }
}