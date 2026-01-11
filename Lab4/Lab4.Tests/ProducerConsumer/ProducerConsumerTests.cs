using Lab4.Core.ProducerConsumer;

namespace Lab4.Tests.ProducerConsumer;

public class ProducerConsumerTests
{
    [Fact]
    public void Producer_ShouldHaveCorrectName()
    {
        // Arrange
        var buffer = new BoundedBuffer<Product>(10);

        // Act
        var producer = new Producer("TestProducer", buffer);

        // Assert
        Assert.Equal("TestProducer", producer.Name);
    }

    [Fact]
    public void Consumer_ShouldHaveCorrectName()
    {
        // Arrange
        var buffer = new BoundedBuffer<Product>(10);

        // Act
        var consumer = new Consumer("TestConsumer", buffer);

        // Assert
        Assert.Equal("TestConsumer", consumer.Name);
    }

    [Fact]
    public async Task Producer_ShouldProduceItems()
    {
        // Arrange
        Product.ResetCounter();
        var buffer = new BoundedBuffer<Product>(10);
        var producer = new Producer("TestProducer", buffer);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        // Act
        await producer.ProduceAsync(5, 10, 50, cts.Token);

        // Assert
        Assert.Equal(5, producer.ItemsProduced);
        Assert.Equal(5, buffer.Count);
    }

    [Fact]
    public async Task Consumer_ShouldConsumeItems()
    {
        // Arrange
        Product.ResetCounter();
        var buffer = new BoundedBuffer<Product>(10);
        buffer.Add(new Product("Test"));
        buffer.Add(new Product("Test"));
        buffer.Add(new Product("Test"));

        var consumer = new Consumer("TestConsumer", buffer);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        // Act
        await consumer.ConsumeAsync(3, 10, 50, cts.Token);

        // Assert
        Assert.Equal(3, consumer.ItemsConsumed);
        Assert.Equal(0, buffer.Count);
    }

    [Fact]
    public async Task ProducerConsumer_ShouldWorkTogether()
    {
        // Arrange
        Product.ResetCounter();
        var buffer = new BoundedBuffer<Product>(5);
        var producer = new Producer("Producer", buffer);
        var consumer = new Consumer("Consumer", buffer);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        // Act
        var produceTask = producer.ProduceAsync(10, 20, 50, cts.Token);
        var consumeTask = consumer.ConsumeAsync(10, 30, 60, cts.Token);

        await Task.WhenAll(produceTask, consumeTask);

        // Assert
        Assert.Equal(10, producer.ItemsProduced);
        Assert.Equal(10, consumer.ItemsConsumed);
        Assert.Equal(0, buffer.Count);
    }

    [Fact]
    public async Task MultipleProducersAndConsumers_ShouldWorkCorrectly()
    {
        // Arrange
        Product.ResetCounter();
        var buffer = new BoundedBuffer<Product>(10);

        var producer1 = new Producer("P1", buffer);
        var producer2 = new Producer("P2", buffer);
        var consumer1 = new Consumer("C1", buffer);
        var consumer2 = new Consumer("C2", buffer);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        // Act
        var tasks = new[]
        {
            producer1.ProduceAsync(10, 10, 30, cts.Token),
            producer2.ProduceAsync(10, 10, 30, cts.Token),
            consumer1.ConsumeAsync(10, 20, 40, cts.Token),
            consumer2.ConsumeAsync(10, 20, 40, cts.Token)
        };

        await Task.WhenAll(tasks);

        // Assert
        var totalProduced = producer1.ItemsProduced + producer2.ItemsProduced;
        var totalConsumed = consumer1.ItemsConsumed + consumer2.ItemsConsumed;

        Assert.Equal(20, totalProduced);
        Assert.Equal(20, totalConsumed);
    }

    [Fact]
    public void Product_ShouldTrackLifecycle()
    {
        // Arrange
        Product.ResetCounter();
        var product = new Product("TestProducer");

        // Act
        product.MarkConsumed("TestConsumer");

        // Assert
        Assert.Equal("TestProducer", product.ProducerName);
        Assert.Equal("TestConsumer", product.ConsumerName);
        Assert.NotNull(product.ConsumedAt);
        Assert.NotNull(product.LifeTime);
    }
}