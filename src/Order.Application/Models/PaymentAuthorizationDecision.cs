namespace Order.Application.Models;

public sealed record PaymentAuthorizationDecision(bool Authorized, string TransactionId = "");
