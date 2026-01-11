using System.Collections.Concurrent;

namespace Lab4.Core.ProducerConsumer;

public class BoundedBuffer<T>
{
    private readonly BlockingCollection<T> _buffer;

    public int Capacity { get; }
    public int Count => _buffer.Count;
    public bool IsFull => _buffer.Count >= Capacity;
    public bool IsEmpty => _buffer.Count == 0;
    public bool IsCompleted => _buffer.IsCompleted;

    public event Action<T>? OnItemAdded;
    public event Action<T>? OnItemTaken;

    public BoundedBuffer(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Вместимость должна быть положительной", nameof(capacity));

        Capacity = capacity;
        _buffer = new BlockingCollection<T>(new ConcurrentQueue<T>(), capacity);
    }

    public void Add(T item, CancellationToken ct = default)
    {
        _buffer.Add(item, ct);
        OnItemAdded?.Invoke(item);
    }

    public bool TryAdd(T item)
    {
        var result = _buffer.TryAdd(item);
        if (result)
        {
            OnItemAdded?.Invoke(item);
        }
        return result;
    }

    public T Take(CancellationToken ct = default)
    {
        var item = _buffer.Take(ct);
        OnItemTaken?.Invoke(item);
        return item;
    }

    public bool TryTake(out T? item)
    {
        var result = _buffer.TryTake(out item);
        if (result && item != null)
        {
            OnItemTaken?.Invoke(item);
        }
        return result;
    }

    public void CompleteAdding()
    {
        _buffer.CompleteAdding();
    }

    public IEnumerable<T> GetConsumingEnumerable(CancellationToken ct = default)
    {
        return _buffer.GetConsumingEnumerable(ct);
    }
}

public class BoundedBufferManual<T>
{
    private readonly Queue<T> _queue = new();
    private readonly object _lock = new();
    private readonly SemaphoreSlim _emptySlots;
    private readonly SemaphoreSlim _fullSlots;

    public int Capacity { get; }
    public int Count
    {
        get
        {
            lock (_lock) return _queue.Count;
        }
    }

    public BoundedBufferManual(int capacity)
    {
        Capacity = capacity;
        _emptySlots = new SemaphoreSlim(capacity, capacity); // Свободные места
        _fullSlots = new SemaphoreSlim(0, capacity);          // Занятые места
    }

    public async Task AddAsync(T item, CancellationToken ct = default)
    {
        // Ждём свободное место
        await _emptySlots.WaitAsync(ct);

        lock (_lock)
        {
            _queue.Enqueue(item);
        }

        // Сигнализируем о новом элементе
        _fullSlots.Release();
    }

    public async Task<T> TakeAsync(CancellationToken ct = default)
    {
        // Ждём элемент
        await _fullSlots.WaitAsync(ct);

        T item;
        lock (_lock)
        {
            item = _queue.Dequeue();
        }

        // Освобождаем место
        _emptySlots.Release();

        return item;
    }
}