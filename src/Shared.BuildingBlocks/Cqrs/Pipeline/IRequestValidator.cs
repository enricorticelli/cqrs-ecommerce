namespace Shared.BuildingBlocks.Cqrs.Pipeline;

public interface IRequestValidator<in TRequest>
{
    IReadOnlyDictionary<string, string[]> Validate(TRequest request);
}
