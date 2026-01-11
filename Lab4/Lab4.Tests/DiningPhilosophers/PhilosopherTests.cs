using Lab4.Core.DiningPhilosophers;

namespace Lab4.Tests.DiningPhilosophers;

public class PhilosopherTests
{
    [Fact]
    public void Philosopher_ShouldHaveCorrectIdAndName()
    {
        // Arrange
        var leftFork = new Fork(0);
        var rightFork = new Fork(1);

        // Act
        var philosopher = new Philosopher(3, leftFork, rightFork);

        // Assert
        Assert.Equal(3, philosopher.Id);
        Assert.Equal("Философ 3", philosopher.Name);
    }

    [Fact]
    public void Philosopher_InitialState_ShouldBeThinking()
    {
        // Arrange
        var leftFork = new Fork(0);
        var rightFork = new Fork(1);

        // Act
        var philosopher = new Philosopher(0, leftFork, rightFork);

        // Assert
        Assert.Equal(PhilosopherState.Thinking, philosopher.State);
    }

    [Fact]
    public void Philosopher_InitialMealsEaten_ShouldBeZero()
    {
        // Arrange
        var leftFork = new Fork(0);
        var rightFork = new Fork(1);

        // Act
        var philosopher = new Philosopher(0, leftFork, rightFork);

        // Assert
        Assert.Equal(0, philosopher.MealsEaten);
    }

    [Fact]
    public async Task Philosopher_ThinkAsync_ShouldSetStateToThinking()
    {
        // Arrange
        var leftFork = new Fork(0);
        var rightFork = new Fork(1);
        var philosopher = new Philosopher(0, leftFork, rightFork);

        // Act
        await philosopher.ThinkAsync(10, 20);

        // Assert
        Assert.Equal(PhilosopherState.Thinking, philosopher.State);
    }

    [Fact]
    public async Task Philosopher_EatAsync_ShouldIncrementMealsEaten()
    {
        // Arrange
        var leftFork = new Fork(0);
        var rightFork = new Fork(1);
        var philosopher = new Philosopher(0, leftFork, rightFork);

        // Act
        await philosopher.EatAsync(10, 20);

        // Assert
        Assert.Equal(1, philosopher.MealsEaten);
    }

    [Fact]
    public async Task Philosopher_DineSafelyAsync_ShouldEatMultipleTimes()
    {
        // Arrange
        var leftFork = new Fork(0);
        var rightFork = new Fork(1);
        var philosopher = new Philosopher(0, leftFork, rightFork);
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));

        // Act
        await philosopher.DineSafelyAsync(cts.Token);

        // Assert
        Assert.True(philosopher.MealsEaten > 0, "Философ должен был поесть хотя бы раз");
    }
}