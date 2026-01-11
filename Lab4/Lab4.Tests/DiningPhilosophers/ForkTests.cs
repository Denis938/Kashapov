using Lab4.Core.DiningPhilosophers;

namespace Lab4.Tests.DiningPhilosophers;

public class ForkTests
{
    [Fact]
    public void Fork_ShouldHaveCorrectId()
    {
        // Arrange & Act
        var fork = new Fork(42);

        // Assert
        Assert.Equal(42, fork.Id);
    }

    [Fact]
    public void Fork_InitiallyNotInUse()
    {
        // Arrange
        var fork = new Fork(0);

        // Assert
        Assert.False(fork.IsInUse);
    }

    [Fact]
    public async Task Fork_PickUpAsync_ShouldMarkAsInUse()
    {
        // Arrange
        var fork = new Fork(0);

        // Act
        await fork.PickUpAsync();

        // Assert
        Assert.True(fork.IsInUse);
    }

    [Fact]
    public async Task Fork_PutDown_ShouldMarkAsNotInUse()
    {
        // Arrange
        var fork = new Fork(0);
        await fork.PickUpAsync();

        // Act
        fork.PutDown();

        // Assert
        Assert.False(fork.IsInUse);
    }

    [Fact]
    public void Fork_TryPickUp_ShouldReturnTrueWhenAvailable()
    {
        // Arrange
        var fork = new Fork(0);

        // Act
        var result = fork.TryPickUp(100);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Fork_TryPickUp_ShouldReturnFalseWhenInUse()
    {
        // Arrange
        var fork = new Fork(0);
        await fork.PickUpAsync();

        // Act
        var result = fork.TryPickUp(50);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Fork_CanBePickedUpAfterPutDown()
    {
        // Arrange
        var fork = new Fork(0);

        // Act
        await fork.PickUpAsync();
        fork.PutDown();
        var result = fork.TryPickUp(100);

        // Assert
        Assert.True(result);
    }
}