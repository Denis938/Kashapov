namespace Lab4.Core.ProducerConsumer;

public class Consumer
{
    private readonly BoundedBuffer<Product> _buffer;
    private readonly Random _random = new();

    public string Name { get; }
    public int ItemsConsumed { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<Consumer, Product>? OnProductConsumed;
    public event Action<Consumer, string>? OnLog;

    public Consumer(string name, BoundedBuffer<Product> buffer)
    {
        Name = name;
        _buffer = buffer;
    }

    public async Task ConsumeAsync(int itemsToConsume, int minDelayMs, int maxDelayMs, CancellationToken ct)
    {
        IsRunning = true;
        Log($"начал работу, нужно потребить: {itemsToConsume}");

        for (int i = 0; i < itemsToConsume && !ct.IsCancellationRequested; i++)
        {
            try
            {
                Log($"жду продукт... (буфер: {_buffer.Count}/{_buffer.Capacity})");

                var product = _buffer.Take(ct);
                product.MarkConsumed(Name);
                ItemsConsumed++;

                OnProductConsumed?.Invoke(this, product);
                Log($"потребил {product}");

                // Симуляция времени потребления
                await Task.Delay(_random.Next(minDelayMs, maxDelayMs), ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        IsRunning = false;
        Log($"закончил работу, потреблено: {ItemsConsumed}");
    }

    public async Task ConsumeIndefinitelyAsync(int minDelayMs, int maxDelayMs, CancellationToken ct)
    {
        IsRunning = true;
        Log("начал бесконечное потребление");

        try
        {
            foreach (var product in _buffer.GetConsumingEnumerable(ct))
            {
                product.MarkConsumed(Name);
                ItemsConsumed++;

                OnProductConsumed?.Invoke(this, product);
                Log($"потребил {product} (всего: {ItemsConsumed})");

                await Task.Delay(_random.Next(minDelayMs, maxDelayMs), ct);
            }
        }
        catch (OperationCanceledException)
        {
            // Нормальное завершение
        }

        IsRunning = false;
        Log($"остановлен, потреблено: {ItemsConsumed}");
    }

    private void Log(string message)
    {
        var logMessage = $"[{DateTime.Now:HH:mm:ss.fff}] [{Name}] {message}";
        Console.WriteLine(logMessage);
        OnLog?.Invoke(this, message);
    }
}