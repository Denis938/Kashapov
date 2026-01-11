namespace Lab4.Core.DiningPhilosophers;

/// Стол философов с реализацией, приводящей к deadlock(намеренно содержит ошибку)
public class DiningTableDeadlock
{
    private readonly Fork[] _forks;
    private readonly Philosopher[] _philosophers;
    private readonly Thread[] _threads;

    public int PhilosopherCount { get; }

    public DiningTableDeadlock(int philosopherCount = 5)
    {
        PhilosopherCount = philosopherCount;

        // Создаём вилки
        _forks = new Fork[philosopherCount];
        for (int i = 0; i < philosopherCount; i++)
        {
            _forks[i] = new Fork(i);
        }

        // Создаём философов
        _philosophers = new Philosopher[philosopherCount];
        for (int i = 0; i < philosopherCount; i++)
        {
            var leftFork = _forks[i];
            var rightFork = _forks[(i + 1) % philosopherCount];
            _philosophers[i] = new Philosopher(i, leftFork, rightFork);
        }

        _threads = new Thread[philosopherCount];
    }

    public IReadOnlyList<Philosopher> Philosophers => _philosophers;

    public void StartWithDeadlock(CancellationToken ct)
    {
        Console.WriteLine("ЗАПУСК ВЕРСИИ С DEADLOCK");
        Console.WriteLine("Все философы будут пытаться взять сначала левую, потом правую вилку.");
        Console.WriteLine("Это приведёт к deadlock!\n");

        for (int i = 0; i < PhilosopherCount; i++)
        {
            int index = i;
            _threads[i] = new Thread(() => _philosophers[index].DineWithDeadlock(ct))
            {
                Name = $"Philosopher-{i}"
            };
            _threads[i].Start();
        }
    }

    public void WaitAll(int timeoutMs = 5000)
    {
        foreach (var thread in _threads)
        {
            thread?.Join(timeoutMs);
        }
    }

    public bool IsDeadlocked()
    {
        return _philosophers.All(p => p.State == PhilosopherState.Hungry);
    }

    public string GetStatistics()
    {
        var stats = _philosophers
            .Select(p => $"{p.Name}: съел {p.MealsEaten} раз")
            .ToList();

        return string.Join("\n", stats);
    }
}