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
}