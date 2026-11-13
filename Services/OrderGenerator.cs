using Variant20.Models;

namespace Variant20.Services;

public static class OrderGenerator
{
    // Каждый 10-й заказ делаем "битым" (отрицательная сумма), чтобы потом проверить ошибки.
    public static List<Order> Generate(int count, int seed = 42)
    {
        var random = new Random(seed);
        var orders = new List<Order>(count);

        for (int i = 1; i <= count; i++)
        {
            bool isInvalid = i % 10 == 0;
            orders.Add(new Order
            {
                Id = i,
                CustomerName = $"Customer_{random.Next(1, 100)}",
                Amount = isInvalid ? -100m : random.Next(100, 5000),
                Quantity = isInvalid ? 0 : random.Next(1, 5),
            });
        }
        return orders;
    }
}