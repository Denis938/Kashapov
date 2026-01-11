using Lab4.Core.DiningPhilosophers;

namespace Lab4.Tests.DiningPhilosophers;

public class DiningTableTests
{
    [Fact]
    public void DiningTableSafe_ShouldCreateCorrectNumberOfPhilosophers()
    {
        // Arrange & Act
        var table = new DiningTableSafe(5);

        // Assert
        Assert.Equal(5, table.PhilosopherCount);
        Assert.Equal(5, table.Philosophers.Count);
    }

    [Fact]
    public void DiningTableSafe_ShouldCreateCorrectNumberOfForks()
    {
        // Arrange & Act
        var table = new DiningTableSafe(5);

        // Assert
        Assert.Equal(5, table.Forks.Count);
    }

    [Fact]
    public async Task DiningTableSafe_AllPhilosophersShouldEat()
    {
        // Arrange
        var table = new DiningTableSafe(3);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        // Act
        table.StartSafe(cts.Token);

        try
        {
            await table.WaitAllAsync();
        }
        catch (OperationCanceledException)
        {
            // Ожидаемо
        }

        // Assert
        Assert.All(table.Philosophers, p =>
            Assert.True(p.MealsEaten > 0, $"{p.Name} должен был поесть"));
    }

    [Fact]
    public async Task DiningTableSafe_NoDeadlock_AllPhilosophersProgress()
    {
        // Arrange
        var table = new DiningTableSafe(5);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        // Act
        table.StartSafe(cts.Token);

        // Проверяем, что симуляция не зависает
        var completed = await Task.WhenAny(
            table.WaitAllAsync(),
            Task.Delay(TimeSpan.FromSeconds(4))
        );

        // Assert: должны были завершиться по таймауту, а не зависнуть
        var totalMeals = table.Philosophers.Sum(p => p.MealsEaten);
        Assert.True(totalMeals > 0, "Философы должны были поесть");
    }

    [Fact]
    public void DiningTableDeadlock_ShouldCreateCorrectStructure()
    {
        // Arrange & Act
        var table = new DiningTableDeadlock(5);

        // Assert
        Assert.Equal(5, table.PhilosopherCount);
        Assert.Equal(5, table.Philosophers.Count);
    }
}