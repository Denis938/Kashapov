namespace Lab4.Core.ProducerConsumer;

public class Producer
{
    private readonly BoundedBuffer<Product> _buffer;
    private readonly Random _random = new();

    public string Name { get; }
    public int ItemsProduced { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<Producer, Product>? OnProductCreated;
    public event Action<Producer, string>? OnLog;

    public Producer(string name, BoundedBuffer<Product> buffer)
    {
        Name = name;
        _buffer = buffer;
    }

    public async Task ProduceAsync(int itemsToProduce, int minDelayMs, int maxDelayMs, CancellationToken ct)
    {
        IsRunning = true;
        Log($"начал работу, нужно произвести: {itemsToProduce}");

        for (int i = 0; i < itemsToProduce && !ct.IsCancellationRequested; i++)
        {
            try
            {
                // Симуляция времени производства
                await Task.Delay(_random.Next(minDelayMs, maxDelayMs), ct);

                var product = new Product(Name);

                Log($"создал {product}, добавляю в буфер (буфер: {_buffer.Count}/{_buffer.Capacity})");

                _buffer.Add(product, ct);
                ItemsProduced++;

                OnProductCreated?.Invoke(this, product);

                Log($"{product} добавлен в буфер");
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        IsRunning = false;
        Log($"закончил работу, произведено: {ItemsProduced}");
    }

    public async Task ProduceIndefinitelyAsync(int minDelayMs, int maxDelayMs, CancellationToken ct)
    {
        IsRunning = true;
        Log("начал бесконечное производство");

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_random.Next(minDelayMs, maxDelayMs), ct);

                var product = new Product(Name);
                _buffer.Add(product, ct);
                ItemsProduced++;

                OnProductCreated?.Invoke(this, product);
                Log($"создал {product} (всего: {ItemsProduced})");
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        IsRunning = false;
        Log($"остановлен, произведено: {ItemsProduced}");
    }

    private void Log(string message)
    {
        var logMessage = $"[{DateTime.Now:HH:mm:ss.fff}] [{Name}] {message}";
        Console.WriteLine(logMessage);
        OnLog?.Invoke(this, message);
    }
}