namespace Lab4.Core.DiningPhilosophers;

public class Philosopher
{
    private readonly Fork _leftFork;
    private readonly Fork _rightFork;
    private readonly Random _random = new();

    public int Id { get; }
    public string Name => $"Философ {Id}";
    public int MealsEaten { get; private set; }
    public PhilosopherState State { get; private set; }

    public event Action<Philosopher, string>? OnStateChanged;

    public Philosopher(int id, Fork leftFork, Fork rightFork)
    {
        Id = id;
        _leftFork = leftFork;
        _rightFork = rightFork;
        State = PhilosopherState.Thinking;
    }

    public async Task ThinkAsync(int minMs = 100, int maxMs = 500)
    {
        State = PhilosopherState.Thinking;
        Log("думает...");
        await Task.Delay(_random.Next(minMs, maxMs));
    }

    public async Task EatAsync(int minMs = 100, int maxMs = 300)
    {
        State = PhilosopherState.Eating;
        Log("ест!");
        await Task.Delay(_random.Next(minMs, maxMs));
        MealsEaten++;
    }

    public void DineWithDeadlock(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            // Думаем
            State = PhilosopherState.Thinking;
            Log("думает...");
            Thread.Sleep(_random.Next(50, 150));

            // Пытаемся взять левую вилку
            State = PhilosopherState.Hungry;
            Log($"пытается взять левую вилку {_leftFork.Id}");

            lock (_leftFork.Lock)
            {
                Log($"взял левую вилку {_leftFork.Id}");

                // Небольшая задержка увеличивает вероятность deadlock
                Thread.Sleep(50);

                Log($"пытается взять правую вилку {_rightFork.Id}");

                lock (_rightFork.Lock)
                {
                    Log($"взял правую вилку {_rightFork.Id}");

                    // Едим
                    State = PhilosopherState.Eating;
                    Log("ест!");
                    Thread.Sleep(_random.Next(50, 100));
                    MealsEaten++;
                }
            }
        }
    }

    public async Task DineSafelyAsync(CancellationToken ct)
    {
        // Определяем порядок захвата вилок (сначала с меньшим ID)
        var firstFork = _leftFork.Id < _rightFork.Id ? _leftFork : _rightFork;
        var secondFork = _leftFork.Id < _rightFork.Id ? _rightFork : _leftFork;

        while (!ct.IsCancellationRequested)
        {
            try
            {
                // Думаем
                await ThinkAsync(50, 150);

                // Голодаем, пытаемся взять вилки
                State = PhilosopherState.Hungry;
                Log($"голоден, пытается взять вилку {firstFork.Id}");

                await firstFork.PickUpAsync(ct);
                Log($"взял первую вилку {firstFork.Id}");

                await secondFork.PickUpAsync(ct);
                Log($"взял вторую вилку {secondFork.Id}");

                // Едим
                await EatAsync(50, 100);

                // Кладём вилки обратно
                secondFork.PutDown();
                firstFork.PutDown();
                Log("положил вилки");
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    /// <summary>
    /// АЛЬТЕРНАТИВНОЕ РЕШЕНИЕ: Используем тайм-аут и повторные попытки.
    /// </summary>
    public async Task DineWithTimeoutAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await ThinkAsync(50, 150);

                State = PhilosopherState.Hungry;

                // Пытаемся взять левую вилку с тайм-аутом
                if (!_leftFork.TryPickUp(100))
                {
                    continue; // Не удалось, пробуем снова
                }

                // Пытаемся взять правую вилку с тайм-аутом
                if (!_rightFork.TryPickUp(100))
                {
                    _leftFork.PutDown(); // Кладём левую обратно
                    await Task.Delay(10); // Небольшая пауза
                    continue;
                }

                // Удалось взять обе вилки
                await EatAsync(50, 100);

                _rightFork.PutDown();
                _leftFork.PutDown();
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private void Log(string message)
    {
        var logMessage = $"[{DateTime.Now:HH:mm:ss.fff}] {Name} {message}";
        Console.WriteLine(logMessage);
        OnStateChanged?.Invoke(this, message);
    }
}

public enum PhilosopherState
{
    Thinking,
    Hungry,
    Eating
}