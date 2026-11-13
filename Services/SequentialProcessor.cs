using Variant20.Models;

namespace Variant20.Services;

public static class SequentialProcessor
{
    // Метод для обработки одного заказа (понадобится и в конвейере)
    public static async Task<OrderResult> ProcessOrderAsync(Order order, int workerId)
    {
        await Task.Delay(10); // Имитация работы

        if (order.Amount <= 0) throw new ArgumentException("Сумма должна быть > 0");
        if (order.Quantity <= 0) throw new ArgumentException("Количество должно быть > 0");

        decimal finalAmount = order.Amount * order.Quantity;
        return new OrderResult(order.Id, order.CustomerName, finalAmount, workerId);
    }

    public static async Task<(List<OrderResult> Results, List<OrderError> Errors, long TimeMs)> ProcessAsync(IEnumerable<Order> orders)
    {
        var results = new List<OrderResult>();
        var errors = new List<OrderError>();
        var sw = System.Diagnostics.Stopwatch.StartNew();

        foreach (var order in orders)
        {
            try
            {
                var result = await ProcessOrderAsync(order, workerId: 1);
                results.Add(result);
            }
            catch (Exception ex)
            {
                errors.Add(new OrderError(order.Id, order.CustomerName, ex.GetType().Name, ex.Message));
            }
        }

        sw.Stop();
        return (results, errors, sw.ElapsedMilliseconds);
    }
}