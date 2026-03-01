using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Shared.BuildingBlocks.Http;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddDefaultProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var correlationId = context.HttpContext.Items.TryGetValue(CorrelationId.ItemKey, out var value)
                    ? value?.ToString()
                    : context.HttpContext.TraceIdentifier;

                context.ProblemDetails.Extensions["correlationId"] = correlationId;
            };
        });

        return services;
    }

    public static IResult ValidationProblem(Dictionary<string, string[]> errors, string detail = "Request validation failed")
    {
        return TypedResults.Problem(new ProblemDetails
        {
            Title = "Validation Error",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail,
            Type = "https://httpstatuses.com/400",
            Extensions = { ["errors"] = errors }
        });
    }

    public static Dictionary<string, string[]> GetValidationErrors<T>(this T model)
    {
        if (model is null)
        {
            return new Dictionary<string, string[]>
            {
                ["request"] = ["The request body is required."]
            };
        }

        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            validationResults,
            validateAllProperties: true);

        return validationResults
            .SelectMany(result =>
            {
                var message = result.ErrorMessage ?? "The value is invalid.";
                return result.MemberNames.Any()
                    ? result.MemberNames.Select(memberName => new KeyValuePair<string, string>(memberName, message))
                    : [new KeyValuePair<string, string>("request", message)];
            })
            .GroupBy(pair => pair.Key)
            .ToDictionary(
                group => group.Key,
                group => group.Select(pair => pair.Value).Distinct().ToArray());
    }
}
