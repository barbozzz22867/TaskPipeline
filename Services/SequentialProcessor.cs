using Variant20.Models;

namespace Variant20.Services;

public static class SequentialProcessor
{
    public static async Task<OrderResult> ProcessOrderAsync(Order order, int workerId)
    {
        await Task.Delay(10); // Имитация задержки

        // Задание 8: Разделение этапов
        OrderValidator.Validate(order);
        return OrderCalculator.Calculate(order, workerId);
    }

    // ... (метод ProcessAsync оставляем без изменений)
    public static async Task<(List<OrderResult> Results, List<OrderError> Errors, long TimeMs)> ProcessAsync(IEnumerable<Order> orders)
    {
        var results = new List<OrderResult>();
        var errors = new List<OrderError>();
        var sw = System.Diagnostics.Stopwatch.StartNew();

        foreach (var order in orders)
        {
            try
            {
                results.Add(await ProcessOrderAsync(order, 1));
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