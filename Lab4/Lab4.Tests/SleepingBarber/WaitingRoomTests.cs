using Lab4.Core.SleepingBarber;

namespace Lab4.Tests.SleepingBarber;

public class WaitingRoomTests
{
    [Fact]
    public void WaitingRoom_ShouldHaveCorrectCapacity()
    {
        // Arrange & Act
        var room = new WaitingRoom(5);

        // Assert
        Assert.Equal(5, room.Capacity);
    }

    [Fact]
    public void WaitingRoom_InitiallyEmpty()
    {
        // Arrange
        var room = new WaitingRoom(5);

        // Assert
        Assert.True(room.IsEmpty);
        Assert.Equal(0, room.CurrentCount);
    }

    [Fact]
    public void WaitingRoom_TryEnter_ShouldAddCustomer()
    {
        // Arrange
        Customer.ResetCounter();
        var room = new WaitingRoom(5);
        var customer = new Customer();

        // Act
        var result = room.TryEnter(customer);

        // Assert
        Assert.True(result);
        Assert.Equal(1, room.CurrentCount);
    }

    [Fact]
    public void WaitingRoom_TryEnter_ShouldRejectWhenFull()
    {
        // Arrange
        Customer.ResetCounter();
        var room = new WaitingRoom(2);
        room.TryEnter(new Customer());
        room.TryEnter(new Customer());

        // Act
        var result = room.TryEnter(new Customer());

        // Assert
        Assert.False(result);
        Assert.True(room.IsFull);
    }

    [Fact]
    public void WaitingRoom_TryGetCustomer_ShouldReturnCustomer()
    {
        // Arrange
        Customer.ResetCounter();
        var room = new WaitingRoom(5);
        var customer = new Customer();
        room.TryEnter(customer);

        // Act
        var result = room.TryGetCustomer(out var retrieved);

        // Assert
        Assert.True(result);
        Assert.Equal(customer.Id, retrieved!.Id);
        Assert.True(room.IsEmpty);
    }

    [Fact]
    public void WaitingRoom_TryGetCustomer_ShouldReturnFalseWhenEmpty()
    {
        // Arrange
        var room = new WaitingRoom(5);

        // Act
        var result = room.TryGetCustomer(out var customer);

        // Assert
        Assert.False(result);
        Assert.Null(customer);
    }

    [Fact]
    public void WaitingRoom_ShouldWorkFIFO()
    {
        // Arrange
        Customer.ResetCounter();
        var room = new WaitingRoom(5);
        var customer1 = new Customer();
        var customer2 = new Customer();
        var customer3 = new Customer();

        room.TryEnter(customer1);
        room.TryEnter(customer2);
        room.TryEnter(customer3);

        // Act & Assert
        room.TryGetCustomer(out var first);
        Assert.Equal(customer1.Id, first!.Id);

        room.TryGetCustomer(out var second);
        Assert.Equal(customer2.Id, second!.Id);

        room.TryGetCustomer(out var third);
        Assert.Equal(customer3.Id, third!.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void WaitingRoom_ShouldThrowOnInvalidCapacity(int invalidCapacity)
    {
        // Assert
        Assert.Throws<ArgumentException>(() => new WaitingRoom(invalidCapacity));
    }
}