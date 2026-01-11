namespace Lab4.Core.DiningPhilosophers;

public class DiningTableSafe
{
    private readonly Fork[] _forks;
    private readonly Philosopher[] _philosophers;
    private readonly List<Task> _tasks = new();

    public int PhilosopherCount { get; }

    public DiningTableSafe(int philosopherCount = 5)
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
    }

    public IReadOnlyList<Philosopher> Philosophers => _philosophers;
    public IReadOnlyList<Fork> Forks => _forks;

    public void StartSafe(CancellationToken ct)
    {
        Console.WriteLine("ЗАПУСК БЕЗОПАСНОЙ ВЕРСИИ");
        Console.WriteLine("Используется упорядочивание ресурсов (всегда берём вилку с меньшим ID первой).\n");

        for (int i = 0; i < PhilosopherCount; i++)
        {
            int index = i;
            _tasks.Add(Task.Run(() => _philosophers[index].DineSafelyAsync(ct)));
        }
    }

    public void StartWithTimeout(CancellationToken ct)
    {
        Console.WriteLine("ЗАПУСК ВЕРСИИ С ТАЙМ-АУТАМИ");
        Console.WriteLine("Если не удалось взять вилку, философ отпускает уже взятую и пробует снова.\n");

        for (int i = 0; i < PhilosopherCount; i++)
        {
            int index = i;
            _tasks.Add(Task.Run(() => _philosophers[index].DineWithTimeoutAsync(ct)));
        }
    }

    public async Task WaitAllAsync()
    {
        await Task.WhenAll(_tasks);
    }

    public string GetStatistics()
    {
        var stats = _philosophers
            .Select(p => $"{p.Name}: съел {p.MealsEaten} раз, состояние: {p.State}")
            .ToList();

        var totalMeals = _philosophers.Sum(p => p.MealsEaten);
        stats.Add($"\nВсего приёмов пищи: {totalMeals}");

        return string.Join("\n", stats);
    }
}