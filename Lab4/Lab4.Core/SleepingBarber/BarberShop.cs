namespace Lab4.Core.SleepingBarber;

public class BarberShop
{
    private readonly Barber _barber;
    private readonly WaitingRoom _waitingRoom;
    private readonly Random _random = new();
    private readonly List<Customer> _allCustomers = new();
    private readonly object _customersLock = new();

    private Task? _barberTask;
    private Task? _customerGeneratorTask;

    public Barber Barber => _barber;
    public WaitingRoom WaitingRoom => _waitingRoom;
    public int TotalCustomers => _allCustomers.Count;
    public int ServedCustomers => _allCustomers.Count(c => c.State == CustomerState.Served);
    public int RejectedCustomers => _allCustomers.Count(c => c.State == CustomerState.Left);

    public event Action<Customer>? OnCustomerArrived;
    public event Action<string>? OnLog;

    public BarberShop(int waitingRoomCapacity = 3)
    {
        _waitingRoom = new WaitingRoom(waitingRoomCapacity);
        _barber = new Barber();

        // Подписываемся на события
        _waitingRoom.OnCustomerEntered += c => Log($"{c} сел в очередь (мест занято: {_waitingRoom.CurrentCount}/{_waitingRoom.Capacity})");
        _waitingRoom.OnCustomerRejected += c =>
        {
            c.Leave();
            Log($"{c} ушёл — нет свободных мест!");
        };
    }

    public void Open(CancellationToken ct)
    {
        Log("ПАРИКМАХЕРСКАЯ ОТКРЫТА");
        Log($"Мест в зале ожидания: {_waitingRoom.Capacity}\n");

        // Запускаем парикмахера
        _barberTask = Task.Run(() => _barber.WorkAsync(_waitingRoom, ct));
    }

    public void StartCustomerGenerator(int minIntervalMs, int maxIntervalMs, CancellationToken ct)
    {
        _customerGeneratorTask = Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_random.Next(minIntervalMs, maxIntervalMs), ct);

                    var customer = new Customer();
                    AddCustomer(customer);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        });
    }

    public bool AddCustomer(Customer customer)
    {
        lock (_customersLock)
        {
            _allCustomers.Add(customer);
        }

        Log($"{customer} пришёл в парикмахерскую");
        OnCustomerArrived?.Invoke(customer);

        var entered = _waitingRoom.TryEnter(customer);

        if (entered)
        {
            // Будим парикмахера, если он спит
            if (_barber.State == BarberState.Sleeping)
            {
                _barber.WakeUp();
            }
        }

        return entered;
    }

    public async Task CloseAsync()
    {
        if (_barberTask != null)
            await _barberTask;

        if (_customerGeneratorTask != null)
            await _customerGeneratorTask;

        Log("\nПАРИКМАХЕРСКАЯ ЗАКРЫТА");
    }

    public string GetStatistics()
    {
        var served = _allCustomers.Where(c => c.State == CustomerState.Served).ToList();
        var avgWait = served.Any()
            ? served.Average(c => c.WaitTime?.TotalMilliseconds ?? 0)
            : 0;

        return $"""
            СТАТИСТИКА
            Всего клиентов: {TotalCustomers}
            Обслужено: {ServedCustomers}
            Ушло (не дождались): {RejectedCustomers}
            Среднее время ожидания: {avgWait:F0}мс
            """;
    }

    private void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        OnLog?.Invoke(message);
    }
}