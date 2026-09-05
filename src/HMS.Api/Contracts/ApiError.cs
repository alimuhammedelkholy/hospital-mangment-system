namespace HMS.Api.Contracts;

public sealed record ApiError(string Code, string Message, string TraceId);
