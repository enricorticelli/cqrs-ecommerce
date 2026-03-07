using System.ComponentModel.DataAnnotations;

namespace Shipping.Api.Contracts;

public sealed record UpdateShipmentStatusRequest([property: Required, StringLength(32)] string Status);
