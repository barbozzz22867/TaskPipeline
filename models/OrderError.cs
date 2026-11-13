namespace Variant20.Models;

public sealed record OrderError(
    int OrderId,
    string CustomerName,
    string ErrorType,
    string Message
);