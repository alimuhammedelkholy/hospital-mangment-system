namespace HMS.Api.Contracts.Errors;

public sealed record ApiError(string Code, string Message, string TraceId);
