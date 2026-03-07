using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Encodings.Web;
using Payment.Api.Contracts;
using Payment.Api.Contracts.Requests;
using Payment.Api.Contracts.Responses;
using Payment.Api.Mappers;
using Payment.Application.Commands;
using Payment.Application.Queries;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Api.Endpoints;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(PaymentRoutes.Base)
            .WithTags("Payment")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

        group.MapPost("/authorize", AuthorizePayment)
            .WithName("AuthorizePayment");
        group.MapGet("/sessions", ListPaymentSessions)
            .WithName("ListPaymentSessions");
        group.MapGet("/sessions/orders/{orderId:guid}", GetPaymentSessionByOrderId)
            .WithName("GetPaymentSessionByOrderId");
        group.MapGet("/sessions/{sessionId:guid}", GetPaymentSessionById)
            .WithName("GetPaymentSessionById");
        group.MapGet("/hosted/{paymentMethod}", RenderHostedPaymentPage)
            .WithName("RenderHostedPaymentPage");
        group.MapPost("/sessions/{sessionId:guid}/authorize", AuthorizePaymentSession)
            .WithName("AuthorizePaymentSession");
        group.MapPost("/sessions/{sessionId:guid}/reject", RejectPaymentSession)
            .WithName("RejectPaymentSession");
        return group;
    }

    private static async Task<Ok<PaymentAuthorizeResponse>> AuthorizePayment(
        AuthorizePaymentRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = PaymentMapper.ToAuthorizePaymentCommand(request);
        var result = await commandDispatcher.ExecuteAsync(command, cancellationToken);
        return TypedResults.Ok(PaymentMapper.ToPaymentAuthorizeResponse(result.OrderId, result.Authorized, result.TransactionId));
    }

    private static async Task<Results<Ok<PaymentSessionResponse>, NotFound>> GetPaymentSessionByOrderId(
        Guid orderId,
        IQueryDispatcher queryDispatcher,
        CancellationToken cancellationToken)
    {
        var session = await queryDispatcher.ExecuteAsync(new GetPaymentSessionByOrderIdQuery(orderId), cancellationToken);
        return session is null ? TypedResults.NotFound() : TypedResults.Ok(PaymentMapper.ToResponse(session));
    }

    private static async Task<Ok<IReadOnlyList<PaymentSessionResponse>>> ListPaymentSessions(
        IQueryDispatcher queryDispatcher,
        int? limit,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit ?? 50, 1, 200);
        var sessions = await queryDispatcher.ExecuteAsync(new GetPaymentSessionsQuery(safeLimit), cancellationToken);
        IReadOnlyList<PaymentSessionResponse> response = sessions.Select(PaymentMapper.ToResponse).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<PaymentSessionResponse>, NotFound>> GetPaymentSessionById(
        Guid sessionId,
        IQueryDispatcher queryDispatcher,
        CancellationToken cancellationToken)
    {
        var session = await queryDispatcher.ExecuteAsync(new GetPaymentSessionByIdQuery(sessionId), cancellationToken);
        return session is null ? TypedResults.NotFound() : TypedResults.Ok(PaymentMapper.ToResponse(session));
    }

    private static Results<ContentHttpResult, ProblemHttpResult> RenderHostedPaymentPage(
        string paymentMethod,
        Guid sessionId,
        Guid orderId,
        string? returnUrl)
    {
        if (!PaymentMethodTypes.IsSupported(paymentMethod))
        {
            return TypedResults.Problem(
                title: "Unsupported payment method",
                detail: $"Allowed values: {string.Join(", ", PaymentMethodTypes.All)}.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var safeReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
            ? "/"
            : Uri.UnescapeDataString(returnUrl);
        var providerLabel = paymentMethod switch
        {
            var x when x.Equals(PaymentMethodTypes.StripeCard, StringComparison.OrdinalIgnoreCase) => "Stripe (Carta)",
            var x when x.Equals(PaymentMethodTypes.PayPal, StringComparison.OrdinalIgnoreCase) => "PayPal",
            var x when x.Equals(PaymentMethodTypes.Satispay, StringComparison.OrdinalIgnoreCase) => "Satispay",
            _ => paymentMethod
        };
        var htmlProviderLabel = HtmlEncoder.Default.Encode(providerLabel);
        var jsReturnUrl = JavaScriptEncoder.Default.Encode(safeReturnUrl);

        var html = $$"""
                     <!DOCTYPE html>
                     <html lang="it">
                     <head>
                       <meta charset="UTF-8" />
                       <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                       <title>Hosted Checkout - {{htmlProviderLabel}}</title>
                       <style>
                         body { font-family: system-ui, -apple-system, Segoe UI, Roboto, sans-serif; background:#f6f7f9; margin:0; }
                         .wrap { max-width: 620px; margin: 32px auto; background:#fff; border:1px solid #d5d9df; border-radius:16px; padding:24px; }
                         .tag { display:inline-block; font-size:12px; background:#eef6ff; color:#0b5fff; padding:4px 8px; border-radius:999px; }
                         .muted { color:#586171; font-size:14px; }
                         .grid { display:grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-top: 16px; }
                         button { border:0; border-radius:10px; padding: 12px 14px; font-weight:600; cursor:pointer; }
                         .ok { background:#0b5fff; color:#fff; }
                         .ko { background:#fff; color:#202734; border:1px solid #d5d9df; }
                         #msg { margin-top: 14px; min-height: 20px; font-size: 13px; color:#3b4250; }
                       </style>
                     </head>
                     <body>
                       <div class="wrap">
                         <span class="tag">Hosted Payment Mock</span>
                         <h1>{{htmlProviderLabel}}</h1>
                         <p class="muted">Pagina esterna simulata. Nessun dato carta viene inviato al backend.</p>
                         <p class="muted">Sessione: <code>{{sessionId:D}}</code><br/>Ordine: <code>{{orderId:D}}</code></p>
                         <div class="grid">
                           <button class="ko" id="rejectBtn">Annulla pagamento</button>
                           <button class="ok" id="authorizeBtn">Conferma pagamento</button>
                         </div>
                         <p id="msg"></p>
                       </div>
                       <script>
                         const paymentBase = window.location.pathname.split('/hosted/')[0];
                         const sessionId = "{{sessionId:D}}";
                         const returnUrl = "{{jsReturnUrl}}";
                         const msg = document.getElementById('msg');

                         async function complete(url, body) {
                           const response = await fetch(url, {
                             method: 'POST',
                             headers: body ? { 'Content-Type': 'application/json' } : undefined,
                             body: body ? JSON.stringify(body) : undefined
                           });

                           if (!response.ok) {
                             throw new Error(`HTTP ${response.status}`);
                           }

                           window.location.assign(returnUrl);
                         }

                         document.getElementById('authorizeBtn')?.addEventListener('click', async () => {
                           msg.textContent = 'Autorizzazione in corso...';
                           try {
                             await complete(`${paymentBase}/sessions/${sessionId}/authorize`);
                           } catch (error) {
                             msg.textContent = `Errore autorizzazione: ${error instanceof Error ? error.message : 'sconosciuto'}`;
                           }
                         });

                         document.getElementById('rejectBtn')?.addEventListener('click', async () => {
                           msg.textContent = 'Annullamento in corso...';
                           try {
                             await complete(`${paymentBase}/sessions/${sessionId}/reject`, { reason: 'Payment cancelled by customer' });
                           } catch (error) {
                             msg.textContent = `Errore annullamento: ${error instanceof Error ? error.message : 'sconosciuto'}`;
                           }
                         });
                       </script>
                     </body>
                     </html>
                     """;

        return TypedResults.Content(html, "text/html; charset=utf-8");
    }

    private static async Task<Results<Ok<PaymentSessionStatusResponse>, NotFound>> AuthorizePaymentSession(
        Guid sessionId,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var found = await commandDispatcher.ExecuteAsync(new AuthorizePaymentSessionCommand(sessionId), cancellationToken);
        if (!found)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(PaymentMapper.ToPaymentSessionStatusResponse(sessionId, "Authorized"));
    }

    private static async Task<Results<Ok<PaymentSessionStatusResponse>, NotFound>> RejectPaymentSession(
        Guid sessionId,
        RejectPaymentSessionRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = PaymentMapper.ToRejectPaymentSessionCommand(sessionId, request);
        var rejected = await commandDispatcher.ExecuteAsync(command, cancellationToken);

        return rejected
            ? TypedResults.Ok(PaymentMapper.ToPaymentSessionStatusResponse(sessionId, "Rejected"))
            : TypedResults.NotFound();
    }
}
