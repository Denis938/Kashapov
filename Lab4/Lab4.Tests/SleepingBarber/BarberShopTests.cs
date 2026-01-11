using Lab4.Core.SleepingBarber;

namespace Lab4.Tests.SleepingBarber;

public class BarberShopTests
{
    [Fact]
    public void BarberShop_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var shop = new BarberShop(waitingRoomCapacity: 5);

        // Assert
        Assert.NotNull(shop.Barber);
        Assert.NotNull(shop.WaitingRoom);
        Assert.Equal(5, shop.WaitingRoom.Capacity);
    }

    [Fact]
    public void BarberShop_Barber_ShouldStartSleeping()
    {
        // Arrange & Act
        var shop = new BarberShop(3);

        // Assert
        Assert.Equal(BarberState.Sleeping, shop.Barber.State);
    }

    [Fact]
    public async Task BarberShop_AddCustomer_ShouldWakeUpBarber()
    {
        // Arrange
        Customer.ResetCounter();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var shop = new BarberShop(3);
        shop.Open(cts.Token);

        // Act
        shop.AddCustomer(new Customer());
        await Task.Delay(300); // Даём время на пробуждение

        // Assert
        Assert.NotEqual(BarberState.Sleeping, shop.Barber.State);
    }

    [Fact]
    public async Task BarberShop_ShouldServeCustomers()
    {
        // Arrange
        Customer.ResetCounter();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var shop = new BarberShop(3);
        shop.Open(cts.Token);

        // Act
        shop.AddCustomer(new Customer());
        shop.AddCustomer(new Customer());

        await Task.Delay(3000); // Даём время на обслуживание

        // Assert
        Assert.True(shop.Barber.CustomersServed > 0, 
            "Парикмахер должен был обслужить хотя бы одного клиента");
    }

    [Fact]
    public void BarberShop_AddCustomer_ShouldRejectWhenFull_Immediate()
    {
        // Arrange
        Customer.ResetCounter();
        var shop = new BarberShop(waitingRoomCapacity: 2);
        
        // НЕ открываем парикмахерскую — парикмахер не работает
        // Поэтому очередь заполнится и не освободится

        // Act
        var result1 = shop.AddCustomer(new Customer());
        var result2 = shop.AddCustomer(new Customer());
        var result3 = shop.AddCustomer(new Customer()); // Должен быть отклонён

        // Assert
        Assert.True(result1, "Первый клиент должен войти");
        Assert.True(result2, "Второй клиент должен войти");
        Assert.False(result3, "Третий клиент должен быть отклонён — нет мест");
    }

    [Fact]
    public void BarberShop_TotalCustomers_ShouldCountAll()
    {
        // Arrange
        Customer.ResetCounter();
        var shop = new BarberShop(2);

        // Act
        shop.AddCustomer(new Customer());
        shop.AddCustomer(new Customer());
        shop.AddCustomer(new Customer()); // Отклонён, но всё равно считается

        // Assert
        Assert.Equal(3, shop.TotalCustomers);
    }

    [Fact]
    public void BarberShop_RejectedCustomers_ShouldCount()
    {
        // Arrange
        Customer.ResetCounter();
        var shop = new BarberShop(1); // Только 1 место!

        // Act
        shop.AddCustomer(new Customer()); // Занимает место
        shop.AddCustomer(new Customer()); // Отклонён
        shop.AddCustomer(new Customer()); // Отклонён

        // Assert
        Assert.Equal(2, shop.RejectedCustomers);
    }

    [Fact]
    public async Task BarberShop_Integration_FullWorkflow()
    {
        // Arrange
        Customer.ResetCounter();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));
        var shop = new BarberShop(2);
        shop.Open(cts.Token);

        // Act — добавляем клиентов с паузами
        shop.AddCustomer(new Customer());
        await Task.Delay(100);
        shop.AddCustomer(new Customer());
        await Task.Delay(100);
        shop.AddCustomer(new Customer());

        // Ждём обслуживания
        await Task.Delay(2500);

        // Assert
        Assert.True(shop.TotalCustomers >= 3);
        Assert.True(shop.ServedCustomers >= 1 || shop.RejectedCustomers >= 1,
            "Должны быть либо обслуженные, либо отклонённые клиенты");
    }
}