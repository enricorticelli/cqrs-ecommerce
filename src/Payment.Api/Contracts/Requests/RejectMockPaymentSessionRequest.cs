using System.ComponentModel.DataAnnotations;

namespace Payment.Api.Contracts.Requests;

public sealed record RejectPaymentSessionRequest([property: StringLength(256)] string? Reason);
