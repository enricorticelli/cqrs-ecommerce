namespace Order.Application;

public sealed record PaymentAuthorizationDecision(bool Authorized, string TransactionId = "");
