using Lab2.Services;

class Program
{
    static void Main()
    {
        Console.WriteLine("Лабораторная работа 2: Сравнение коллекций\n");
        Console.WriteLine($"Количество элементов: 100,000");
        Console.WriteLine($"Количество итераций: 5\n");

        var benchmark = new CollectionBenchmark(elementCount: 100_000, iterations: 5);
        var results = benchmark.RunAllBenchmarks();

        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("РЕЗУЛЬТАТЫ ТЕСТИРОВАНИЯ");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"{"Коллекция",-20} | {"Операция",-25} | {"Время (мс)",10}");
        Console.WriteLine(new string('-', 70));

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }

        Console.WriteLine(new string('=', 70));

        // Группировка по операциям для сравнения
        Console.WriteLine("\nСРАВНЕНИЕ ПО ОПЕРАЦИЯМ\n");

        PrintComparison(results, "Add End");
        PrintComparison(results, "Add Beginning");
        PrintComparison(results, "Remove End");
        PrintComparison(results, "Remove Beginning");
        PrintComparison(results, "Search");
        PrintComparison(results, "Get By Index");

    }

    static void PrintComparison(List<BenchmarkResult> results, string operationContains)
    {
        var filtered = results
            .Where(r => r.Operation.Contains(operationContains))
            .OrderBy(r => r.AverageTimeMs) // по времени  от меньшего к большему
            .ToList();

        if (filtered.Count == 0) return;

        Console.WriteLine($"{operationContains}");
        foreach (var r in filtered)
        {
            Console.WriteLine($"  {r.CollectionType,-20}: {r.AverageTimeMs,10:F4} ms");
        }
        Console.WriteLine();
    }

    
}