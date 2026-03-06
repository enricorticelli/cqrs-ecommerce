namespace Shared.BuildingBlocks.Cqrs.Pipeline;

public sealed class RequestValidationException : Exception
{
    public RequestValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Request validation failed")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
