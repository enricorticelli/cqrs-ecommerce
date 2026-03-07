using System.ComponentModel.DataAnnotations;

namespace Payment.Api.Contracts;

public sealed record RejectPaymentSessionRequest([property: StringLength(256)] string? Reason);
