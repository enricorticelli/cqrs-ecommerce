using System.ComponentModel.DataAnnotations;

namespace Order.Api.Contracts;

public sealed record ManualCancelOrderRequest([property: StringLength(256)] string? Reason);
