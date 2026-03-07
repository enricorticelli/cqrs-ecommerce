namespace Shared.BuildingBlocks.Exceptions;

public sealed class NotFoundAppException(string message) : AppException(message);
