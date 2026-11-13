using Variant20.Models;

namespace Variant20.Services;

public static class OrderCalculator
{
    // Отдельный этап: только расчёт стоимости
    public static OrderResult Calculate(Order order, int workerId)
    {
        decimal finalAmount = order.Amount * order.Quantity;

        // Если сумма заказа превышает 10 000, даём скидку 10%
        if (finalAmount > 10_000m)
        {
            finalAmount *= 0.9m;
        }

        return new OrderResult(order.Id, order.CustomerName, finalAmount, workerId);
    }
}