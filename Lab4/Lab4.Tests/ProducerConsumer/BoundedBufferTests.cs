using Lab4.Core.ProducerConsumer;

namespace Lab4.Tests.ProducerConsumer;

public class BoundedBufferTests
{
    [Fact]
    public void BoundedBuffer_ShouldHaveCorrectCapacity()
    {
        // Arrange & Act
        var buffer = new BoundedBuffer<int>(10);

        // Assert
        Assert.Equal(10, buffer.Capacity);
    }

    [Fact]
    public void BoundedBuffer_InitiallyEmpty()
    {
        // Arrange
        var buffer = new BoundedBuffer<int>(10);

        // Assert
        Assert.True(buffer.IsEmpty);
        Assert.Equal(0, buffer.Count);
    }

    [Fact]
    public void BoundedBuffer_Add_ShouldIncreaseCount()
    {
        // Arrange
        var buffer = new BoundedBuffer<int>(10);

        // Act
        buffer.Add(42);

        // Assert
        Assert.Equal(1, buffer.Count);
        Assert.False(buffer.IsEmpty);
    }

    [Fact]
    public void BoundedBuffer_Take_ShouldDecreaseCount()
    {
        // Arrange
        var buffer = new BoundedBuffer<int>(10);
        buffer.Add(42);

        // Act
        var item = buffer.Take();

        // Assert
        Assert.Equal(42, item);
        Assert.Equal(0, buffer.Count);
    }

    [Fact]
    public void BoundedBuffer_ShouldWorkFIFO()
    {
        // Arrange
        var buffer = new BoundedBuffer<int>(10);

        // Act
        buffer.Add(1);
        buffer.Add(2);
        buffer.Add(3);

        // Assert
        Assert.Equal(1, buffer.Take());
        Assert.Equal(2, buffer.Take());
        Assert.Equal(3, buffer.Take());
    }

    [Fact]
    public void BoundedBuffer_TryAdd_ShouldReturnFalseWhenFull()
    {
        // Arrange
        var buffer = new BoundedBuffer<int>(2);
        buffer.Add(1);
        buffer.Add(2);

        // Act
        var result = buffer.TryAdd(3);

        // Assert
        Assert.False(result);
        Assert.True(buffer.IsFull);
    }

    [Fact]
    public void BoundedBuffer_TryTake_ShouldReturnFalseWhenEmpty()
    {
        // Arrange
        var buffer = new BoundedBuffer<int>(10);

        // Act
        var result = buffer.TryTake(out var item);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void BoundedBuffer_ShouldThrowOnInvalidCapacity(int invalidCapacity)
    {
        // Assert
        Assert.Throws<ArgumentException>(() => new BoundedBuffer<int>(invalidCapacity));
    }

    [Fact]
    public async Task BoundedBuffer_Add_ShouldBlockWhenFull()
    {
        // Arrange
        var buffer = new BoundedBuffer<int>(1);
        buffer.Add(1);
        var addStarted = false;
        var addCompleted = false;

        // Act
        var addTask = Task.Run(() =>
        {
            addStarted = true;
            buffer.Add(2);
            addCompleted = true;
        });

        await Task.Delay(100);
        Assert.True(addStarted);
        Assert.False(addCompleted); // Должен блокироваться

        buffer.Take(); // Освобождаем место
        await Task.Delay(100);

        // Assert
        Assert.True(addCompleted);
    }

    [Fact]
    public async Task BoundedBuffer_Take_ShouldBlockWhenEmpty()
    {
        // Arrange
        var buffer = new BoundedBuffer<int>(10);
        var takeStarted = false;
        var takeCompleted = false;

        // Act
        var takeTask = Task.Run(() =>
        {
            takeStarted = true;
            buffer.Take();
            takeCompleted = true;
        });

        await Task.Delay(100);
        Assert.True(takeStarted);
        Assert.False(takeCompleted); // Должен блокироваться

        buffer.Add(42); // Добавляем элемент
        await Task.Delay(100);

        // Assert
        Assert.True(takeCompleted);
    }
}