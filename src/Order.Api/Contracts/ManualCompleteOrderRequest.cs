using System.ComponentModel.DataAnnotations;

namespace Order.Api.Contracts;

public sealed record ManualCompleteOrderRequest(
    [property: StringLength(64)] string? TrackingCode,
    [property: StringLength(128)] string? TransactionId);
