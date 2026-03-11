namespace Account.Api.Contracts.Responses;

public sealed record PermissionsResponse(string Role, string[] Permissions);
