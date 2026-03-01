using Microsoft.AspNetCore.Http.HttpResults;
using Payment.Application;
using Shared.BuildingBlocks.Contracts;

namespace Payment.Api.Endpoints;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/payments")
            .WithTags("Payment");

        group.MapPost("/authorize", AuthorizePayment)
            .WithName("AuthorizePayment");

        return group;
    }

    private static async Task<Ok<object>> AuthorizePayment(PaymentAuthorizeRequestedV1 request, IPaymentService service, CancellationToken cancellationToken)
    {
        var result = await service.AuthorizeAsync(request, cancellationToken);
        return TypedResults.Ok((object)new { result.OrderId, result.Authorized, result.TransactionId });
    }
}
