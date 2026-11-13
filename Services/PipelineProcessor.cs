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

    public static async Task RunResultsDemoAsync()
    {
        var channel = Channel.CreateBounded<Order>(10);

        // Задание 5: Отдельные коллекции для успехов и ошибок
        var results = new ConcurrentBag<OrderResult>();
        var errors = new ConcurrentBag<OrderError>();

        var workers = Enumerable.Range(1, 4).Select(workerId => Task.Run(async () =>
        {
            await foreach (var order in channel.Reader.ReadAllAsync())
            {
                try
                {
                    var result = await SequentialProcessor.ProcessOrderAsync(order, workerId);
                    results.Add(result); // Успех
                }
                catch (Exception ex)
                {
                    errors.Add(new OrderError(order.Id, order.CustomerName, ex.GetType().Name, ex.Message)); // Ошибка
                }
            }
        })).ToArray();

        // Генерируем 20 заказов (из них 2 будут с ошибкой)
        var orders = OrderGenerator.Generate(20);
        foreach (var order in orders) await channel.Writer.WriteAsync(order);

        channel.Writer.Complete();
        await Task.WhenAll(workers);

        Console.WriteLine($"Успешно: {results.Count}, Ошибок: {errors.Count}");
    }
    public static async Task RunBackpressureDemoAsync()
    {
        // Канал всего на 2 элемента (чтобы быстро заполнился)
        var channel = Channel.CreateBounded<Order>(2);
        var workers = Enumerable.Range(1, 2).Select(_ => Task.Run(async () =>
        {
            await foreach (var order in channel.Reader.ReadAllAsync())
                await Task.Delay(500); // Медленная обработка
        })).ToArray();

        var orders = OrderGenerator.Generate(10);
        Console.WriteLine("Пытаемся записать 10 заказов в очередь (лимит 2)...");

        for (int i = 0; i < orders.Count; i++)
        {
            var writeTask = channel.Writer.WriteAsync(orders[i]);

            // Задание 6: Если запись не завершилась мгновенно — очередь заполнена!
            if (!writeTask.IsCompleted)
            {
                Console.WriteLine($"[BACKPRESSURE] Канал заполнен! Ждем освобождения места для заказа #{orders[i].Id}...");
            }

            await writeTask;
        }

        channel.Writer.Complete();
        await Task.WhenAll(workers);
        Console.WriteLine("Все заказы переданы.");
    }
    public static async Task<(List<OrderResult> Results, List<OrderError> Errors, long TimeMs)> ProcessAsync(IEnumerable<Order> orders, int workerCount)
    {
        var results = new System.Collections.Concurrent.ConcurrentBag<OrderResult>();
        var errors = new System.Collections.Concurrent.ConcurrentBag<OrderError>();
        var channel = System.Threading.Channels.Channel.CreateBounded<Order>(10);
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var workers = Enumerable.Range(1, workerCount).Select(workerId => Task.Run(async () =>
        {
            await foreach (var order in channel.Reader.ReadAllAsync())
            {
                try
                {
                    var result = await SequentialProcessor.ProcessOrderAsync(order, workerId);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    errors.Add(new OrderError(order.Id, order.CustomerName, ex.GetType().Name, ex.Message));
                }
            }
        })).ToArray();

        foreach (var order in orders) await channel.Writer.WriteAsync(order);

        channel.Writer.Complete();
        await Task.WhenAll(workers);
        sw.Stop();

        return (results.ToList(), errors.ToList(), sw.ElapsedMilliseconds);
    }
}