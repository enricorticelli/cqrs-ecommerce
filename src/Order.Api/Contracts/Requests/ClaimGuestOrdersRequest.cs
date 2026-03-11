namespace Order.Api.Contracts.Requests;

public sealed record ClaimGuestOrdersRequest(Guid AuthenticatedUserId, string CustomerEmail);
