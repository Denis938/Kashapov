namespace Lab4.Core.SleepingBarber;

public class Barber
{
    private readonly SemaphoreSlim _customerReady = new(0);
    private readonly object _stateLock = new();
    private readonly Random _random = new();

    public string Name { get; }
    public BarberState State { get; private set; }
    public int CustomersServed { get; private set; }
    public Customer? CurrentCustomer { get; private set; }

    public event Action<Barber, string>? OnStateChanged;

    public Barber(string name = "Парикмахер")
    {
        Name = name;
        State = BarberState.Sleeping;
    }

    public void WakeUp()
    {
        _customerReady.Release();
    }

    public async Task WorkAsync(WaitingRoom waitingRoom, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                // Проверяем, есть ли клиенты
                if (!waitingRoom.TryGetCustomer(out var customer))
                {
                    // Нет клиентов — засыпаем
                    SetState(BarberState.Sleeping);
                    Log("засыпает...");

                    // Ждём сигнала о новом клиенте
                    await _customerReady.WaitAsync(ct);

                    // Просыпаемся
                    SetState(BarberState.Ready);
                    Log("просыпается!");

                    continue;
                }

                // Обслуживаем клиента
                await ServeCustomerAsync(customer, ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        SetState(BarberState.Finished);
        Log("закончил работу");
    }

    private async Task ServeCustomerAsync(Customer customer, CancellationToken ct)
    {
        CurrentCustomer = customer;
        SetState(BarberState.Cutting);
        customer.StartService();

        Log($"стрижёт {customer}");

        // Симуляция стрижки
        var serviceTime = _random.Next(200, 500);
        await Task.Delay(serviceTime, ct);

        customer.EndService();
        CustomersServed++;
        CurrentCustomer = null;

        Log($"закончил стричь {customer} (время: {customer.ServiceDuration?.TotalMilliseconds:F0}мс)");
    }

    private void SetState(BarberState newState)
    {
        lock (_stateLock)
        {
            State = newState;
        }
    }

    private void Log(string message)
    {
        var logMessage = $"[{DateTime.Now:HH:mm:ss.fff}] {Name} {message}";
        Console.WriteLine(logMessage);
        OnStateChanged?.Invoke(this, message);
    }
}

public enum BarberState
{
    Sleeping,
    Ready,
    Cutting,
    Finished
}