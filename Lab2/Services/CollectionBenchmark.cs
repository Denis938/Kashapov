using System.Collections.Immutable;
using System.Diagnostics;

namespace Lab2.Services;

public class CollectionBenchmark
{
    private readonly int _elementCount;
    private readonly int _iterations;

    public CollectionBenchmark(int elementCount = 100_000, int iterations = 5)
    {
        _elementCount = elementCount;
        _iterations = iterations;
    }

    #region List<T> Benchmarks

    public BenchmarkResult ListAddEnd()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var list = new List<int>();
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < _elementCount; j++)
                list.Add(j);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "List<T>",
            Operation = "Add End",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult ListAddBeginning()
    {
        var times = new List<double>();
        int count = _elementCount; 

        for (int i = 0; i < _iterations; i++)
        {
            var list = new List<int>();
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < count; j++)
                list.Insert(0, j);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "List<T>",
            Operation = $"Add Beginning ({count})",
            AverageTimeMs = times.Average(),
            ElementCount = count,
            Iterations = _iterations
        };
    }

    public BenchmarkResult ListAddMiddle()
    {
        var times = new List<double>();
        int count = _elementCount;

        for (int i = 0; i < _iterations; i++)
        {
            var list = new List<int>();
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < count; j++)
                list.Insert(list.Count / 2, j);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "List<T>",
            Operation = $"Add Middle ({count})",
            AverageTimeMs = times.Average(),
            ElementCount = count,
            Iterations = _iterations
        };
    }

    public BenchmarkResult ListRemoveEnd()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var list = Enumerable.Range(0, _elementCount).ToList();
            var sw = Stopwatch.StartNew();
            
            while (list.Count > 0)
                list.RemoveAt(list.Count - 1);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "List<T>",
            Operation = "Remove End",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult ListRemoveBeginning()
    {
        var times = new List<double>();
        int count = _elementCount;

        for (int i = 0; i < _iterations; i++)
        {
            var list = Enumerable.Range(0, count).ToList();
            var sw = Stopwatch.StartNew();
            
            while (list.Count > 0)
                list.RemoveAt(0);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "List<T>",
            Operation = $"Remove Beginning ({count})",
            AverageTimeMs = times.Average(),
            ElementCount = count,
            Iterations = _iterations
        };
    }

    public BenchmarkResult ListSearch()
    {
        var times = new List<double>();
        var list = Enumerable.Range(0, _elementCount).ToList();
        int searchValue = _elementCount - 1; // Худший случай 

        for (int i = 0; i < _iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < 1000; j++)
                _ = list.Contains(searchValue);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "List<T>",
            Operation = "Search (1000 times)",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult ListGetByIndex()
    {
        var times = new List<double>();
        var list = Enumerable.Range(0, _elementCount).ToList();

        for (int i = 0; i < _iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < _elementCount; j++)
                _ = list[j];
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "List<T>",
            Operation = "Get By Index",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    #endregion

    #region LinkedList<T> Benchmarks

    public BenchmarkResult LinkedListAddEnd()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var list = new LinkedList<int>();
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < _elementCount; j++)
                list.AddLast(j);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "LinkedList<T>",
            Operation = "Add End",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult LinkedListAddBeginning()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var list = new LinkedList<int>();
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < _elementCount; j++)
                list.AddFirst(j);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "LinkedList<T>",
            Operation = "Add Beginning",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult LinkedListRemoveEnd()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var list = new LinkedList<int>(Enumerable.Range(0, _elementCount));
            var sw = Stopwatch.StartNew();
            
            while (list.Count > 0)
                list.RemoveLast();
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "LinkedList<T>",
            Operation = "Remove End",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult LinkedListRemoveBeginning()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var list = new LinkedList<int>(Enumerable.Range(0, _elementCount));
            var sw = Stopwatch.StartNew();
            
            while (list.Count > 0)
                list.RemoveFirst();
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "LinkedList<T>",
            Operation = "Remove Beginning",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult LinkedListSearch()
    {
        var times = new List<double>();
        var list = new LinkedList<int>(Enumerable.Range(0, _elementCount));
        int searchValue = _elementCount - 1;

        for (int i = 0; i < _iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < 1000; j++)
                _ = list.Contains(searchValue);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "LinkedList<T>",
            Operation = "Search (1000 times)",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    #endregion

    #region Queue<T> Benchmarks

    public BenchmarkResult QueueEnqueue()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var queue = new Queue<int>();
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < _elementCount; j++)
                queue.Enqueue(j);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "Queue<T>",
            Operation = "Enqueue (Add End)",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult QueueDequeue()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var queue = new Queue<int>(Enumerable.Range(0, _elementCount));
            var sw = Stopwatch.StartNew();
            
            while (queue.Count > 0)
                queue.Dequeue();
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "Queue<T>",
            Operation = "Dequeue (Remove Beginning)",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult QueueSearch()
    {
        var times = new List<double>();
        var queue = new Queue<int>(Enumerable.Range(0, _elementCount));
        int searchValue = _elementCount - 1;

        for (int i = 0; i < _iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < 1000; j++)
                _ = queue.Contains(searchValue);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "Queue<T>",
            Operation = "Search (1000 times)",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    #endregion

    #region Stack<T> Benchmarks

    public BenchmarkResult StackPush()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var stack = new Stack<int>();
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < _elementCount; j++)
                stack.Push(j);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "Stack<T>",
            Operation = "Push (Add End)",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult StackPop()
    {
        var times = new List<double>();

        for (int i = 0; i < _iterations; i++)
        {
            var stack = new Stack<int>(Enumerable.Range(0, _elementCount));
            var sw = Stopwatch.StartNew();
            
            while (stack.Count > 0)
                stack.Pop();
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "Stack<T>",
            Operation = "Pop (Remove End)",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult StackSearch()
    {
        var times = new List<double>();
        var stack = new Stack<int>(Enumerable.Range(0, _elementCount));
        int searchValue = 0; // Худший случай для Stack

        for (int i = 0; i < _iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < 1000; j++)
                _ = stack.Contains(searchValue);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "Stack<T>",
            Operation = "Search (1000 times)",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    #endregion

    #region ImmutableList<T> Benchmarks

    public BenchmarkResult ImmutableListAddEnd()
    {
        var times = new List<double>();
        int count = _elementCount; // Очень медленно, уменьшаем

        for (int i = 0; i < _iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < count; j++)
                list = list.Add(j);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "ImmutableList<T>",
            Operation = $"Add End ({count})",
            AverageTimeMs = times.Average(),
            ElementCount = count,
            Iterations = _iterations
        };
    }

    public BenchmarkResult ImmutableListAddBeginning()
    {
        var times = new List<double>();
        int count = _elementCount;

        for (int i = 0; i < _iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < count; j++)
                list = list.Insert(0, j);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "ImmutableList<T>",
            Operation = $"Add Beginning ({count})",
            AverageTimeMs = times.Average(),
            ElementCount = count,
            Iterations = _iterations
        };
    }

    public BenchmarkResult ImmutableListSearch()
    {
        var times = new List<double>();
        var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, _elementCount));
        int searchValue = _elementCount - 1;

        for (int i = 0; i < _iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < 1000; j++)
                _ = list.Contains(searchValue);
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "ImmutableList<T>",
            Operation = "Search (100 times)",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    public BenchmarkResult ImmutableListGetByIndex()
    {
        var times = new List<double>();
        var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, _elementCount));

        for (int i = 0; i < _iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            
            for (int j = 0; j < _elementCount; j++)
                _ = list[j];
            
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = "ImmutableList<T>",
            Operation = "Get By Index",
            AverageTimeMs = times.Average(),
            ElementCount = _elementCount,
            Iterations = _iterations
        };
    }

    #endregion

    #region Run All Benchmarks

    public List<BenchmarkResult> RunAllBenchmarks()
    {
        var results = new List<BenchmarkResult>();

        Console.WriteLine("Running List<T> benchmarks...");
        results.Add(ListAddEnd());
        results.Add(ListAddBeginning());
        results.Add(ListAddMiddle());
        results.Add(ListRemoveEnd());
        results.Add(ListRemoveBeginning());
        results.Add(ListSearch());
        results.Add(ListGetByIndex());

        Console.WriteLine("Running LinkedList<T> benchmarks...");
        results.Add(LinkedListAddEnd());
        results.Add(LinkedListAddBeginning());
        results.Add(LinkedListRemoveEnd());
        results.Add(LinkedListRemoveBeginning());
        results.Add(LinkedListSearch());

        Console.WriteLine("Running Queue<T> benchmarks...");
        results.Add(QueueEnqueue());
        results.Add(QueueDequeue());
        results.Add(QueueSearch());

        Console.WriteLine("Running Stack<T> benchmarks...");
        results.Add(StackPush());
        results.Add(StackPop());
        results.Add(StackSearch());

        Console.WriteLine("Running ImmutableList<T> benchmarks...");
        results.Add(ImmutableListAddEnd());
        results.Add(ImmutableListAddBeginning());
        results.Add(ImmutableListSearch());
        results.Add(ImmutableListGetByIndex());

        return results;
    }

    #endregion
}