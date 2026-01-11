namespace Lab4.Core.DiningPhilosophers;

public class Fork
{
    private readonly object _lock = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _isInUse;

    public int Id { get; }

    public Fork(int id)
    {
        Id = id;
    }

    public object Lock => _lock;

    public bool TryPickUp(int timeoutMs = 0)
    {
        return _semaphore.Wait(timeoutMs);
    }

    public async Task PickUpAsync(CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        _isInUse = true;
    }

    public void PutDown()
    {
        _isInUse = false;
        _semaphore.Release();
    }

    public bool IsInUse => _isInUse;

    public void Reset()
    {
        _isInUse = false;
        // Пересоздаём семафор для чистого состояния
        while (_semaphore.CurrentCount < 1)
        {
            _semaphore.Release();
        }
    }
}