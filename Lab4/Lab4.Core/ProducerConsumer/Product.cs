namespace Lab4.Core.ProducerConsumer;

public class Product
{
    private static int _idCounter;

    public int Id { get; }
    public string ProducerName { get; }
    public DateTime CreatedAt { get; }
    public DateTime? ConsumedAt { get; private set; }
    public string? ConsumerName { get; private set; }

    public Product(string producerName)
    {
        Id = Interlocked.Increment(ref _idCounter);
        ProducerName = producerName;
        CreatedAt = DateTime.Now;
    }

    public void MarkConsumed(string consumerName)
    {
        ConsumedAt = DateTime.Now;
        ConsumerName = consumerName;
    }

    public TimeSpan? LifeTime => ConsumedAt - CreatedAt;

    public override string ToString() => $"Продукт #{Id} (от {ProducerName})";

    public static void ResetCounter() => _idCounter = 0;
}