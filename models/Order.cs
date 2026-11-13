namespace Variant20.Models;

public sealed class Order
{
    public int Id { get; init; }
    public string CustomerName { get; init; } = "";
    public decimal Amount { get; init; }
    public int Quantity { get; init; }
}