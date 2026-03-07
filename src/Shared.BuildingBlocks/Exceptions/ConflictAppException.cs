namespace Shared.BuildingBlocks.Exceptions;

public sealed class ConflictAppException(string message) : AppException(message);
