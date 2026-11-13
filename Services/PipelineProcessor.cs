using System.Collections.Concurrent;
using System.Threading.Channels;
using Variant20.Models;

namespace Variant20.Services;

public static class PipelineProcessor
{
    public static async Task RunChannelDemoAsync()
    {
        // Задание 3: Создаем ОГРАНИЧЕННЫЙ канал
        var channel = Channel.CreateBounded<Order>(new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.Wait, // Если полон — ждать
            SingleReader = false,
            SingleWriter = true
        });

        Console.WriteLine("Канал вместимостью 10 создан.");

        // Проверим, что он пуст
        Console.WriteLine($"Есть ли данные для чтения? {channel.Reader.TryRead(out _)}");

        await Task.CompletedTask;
    }

    public static async Task RunWorkersDemoAsync()
    {
        var channel = Channel.CreateBounded<Order>(10);
        var processedCount = new ConcurrentBag<int>(); // Потокобезопасная коллекция

        // Задание 4: Запускаем 3 воркера
        var workers = Enumerable.Range(1, 3).Select(workerId => Task.Run(async () =>
        {
            // Пока просто читаем из канала
            await foreach (var order in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"[Воркер {workerId}] получил заказ #{order.Id}");
                processedCount.Add(order.Id);
                await Task.Delay(50); // Имитация работы
            }
        })).ToArray();

        // Отправляем 5 заказов
        for (int i = 1; i <= 5; i++)
        {
            await channel.Writer.WriteAsync(new Order { Id = i });
        }

        // Закрываем канал, чтобы воркеры завершились (иначе зависнут)
        channel.Writer.Complete();
        await Task.WhenAll(workers);

        Console.WriteLine($"Всего обработано: {processedCount.Count}");
    }
}