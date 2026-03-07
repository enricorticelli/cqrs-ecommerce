using Microsoft.AspNetCore.Http;

namespace Shared.BuildingBlocks.Api.Correlation;

public static class CorrelationIdResolver
{
    public static string Resolve(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("x-correlation-id", out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value.ToString();
        }

        return httpContext.TraceIdentifier;
    }
}
