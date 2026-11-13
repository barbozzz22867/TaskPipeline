using Variant20.Models;

namespace Variant20.Services;

public static class OrderValidator
{
    // Отдельный этап: только проверка данных
    public static void Validate(Order order)
    {
        if (order.Amount <= 0)
            throw new ArgumentException("Сумма заказа должна быть больше нуля.");

        if (order.Quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля.");
    }
}