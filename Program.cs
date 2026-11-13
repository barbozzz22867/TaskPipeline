using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using Variant20.Models;
using Variant20.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Variant20;

internal static class Program
{
    private static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Зачётный проект 20: Конвейер обработки заказов");
        Console.WriteLine(new string('=', 64));

        // Стартовый сценарий нужен для проверки структуры проекта.
        // Реализацию индивидуальных требований выполняйте в ProjectService.
        var service = new ProjectService();
        await service.RunAsync();

        Console.WriteLine();
        Console.WriteLine("Нажмите Enter для завершения.");
        Console.ReadLine();
    }
}

public sealed class ProjectService
{
    public async Task RunAsync()
    {
        // TODO: перенесите предметную логику в отдельные методы и классы.
        // TODO: не размещайте весь проект в Main.
        // TODO 1: Создать модель заказа и генератор входных заказов.
        var orders = OrderGenerator.Generate(15);
        Console.WriteLine($"Сгенерировано заказов: {orders.Count}");
        Console.WriteLine($"Первый заказ: {orders[0].CustomerName}, Сумма: {orders[0].Amount}");
        Console.WriteLine($"Десятый заказ (должен быть битым): Сумма: {orders[9].Amount}");
        // TODO 2: Реализовать последовательную обработку.
        var seq = await SequentialProcessor.ProcessAsync(orders);
        Console.WriteLine($"Успешно: {seq.Results.Count}, Ошибок: {seq.Errors.Count}");
        // TODO 3: Создать bounded Channel для передачи заказов worker-ам.
        await PipelineProcessor.RunChannelDemoAsync();
        // TODO 4: Запустить фиксированное число worker-ов.
        // TODO 5: Собирать успешные результаты и ошибки отдельно.
        // TODO 6: Показать backpressure при заполнении очереди.
        // TODO 7: Корректно завершать writer и дожидаться worker-ов.

        // TODO 8: Добавить отдельные этапы валидации и обработки.
        // TODO 9: Сравнить производительность при разном числе worker-ов.
        // TODO 10: Сохранять сводку в JSON.

        Console.WriteLine("Стартовый проект собран. Реализуйте задания варианта.");
        await Task.CompletedTask;
    }
}