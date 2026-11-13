namespace Variant20.Models;

public sealed record OrderResult(
    int OrderId,
    string CustomerName,
    decimal FinalAmount,
    int WorkerId
);