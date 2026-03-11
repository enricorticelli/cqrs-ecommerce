using System.Security.Claims;

namespace Account.Api.Mappers;

public static class AccountContextMapper
{
    public static Guid GetRequiredUserId(this HttpContext context)
    {
        var raw = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("Missing subject claim.");

        if (!Guid.TryParse(raw, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid subject claim.");
        }

        return userId;
    }

    public static string GetRequiredRealm(this HttpContext context)
    {
        return context.User.FindFirstValue("realm")
            ?? throw new UnauthorizedAccessException("Missing realm claim.");
    }
}
