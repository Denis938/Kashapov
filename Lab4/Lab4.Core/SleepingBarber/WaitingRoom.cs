using System.Collections.Concurrent;

namespace Lab4.Core.SleepingBarber;

public class WaitingRoom
{
    private readonly ConcurrentQueue<Customer> _queue = new();
    private readonly SemaphoreSlim _seats;
    private readonly int _capacity;
    private int _currentCount;

    public int Capacity => _capacity;
    public int CurrentCount => _currentCount;
    public bool IsFull => _currentCount >= _capacity;
    public bool IsEmpty => _currentCount == 0;

    public event Action<Customer>? OnCustomerEntered;
    public event Action<Customer>? OnCustomerLeft;
    public event Action<Customer>? OnCustomerRejected;

    public WaitingRoom(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Вместимость должна быть положительной", nameof(capacity));

        _capacity = capacity;
        _seats = new SemaphoreSlim(capacity, capacity);
    }

    public bool TryEnter(Customer customer)
    {
        // Пытаемся занять место (без ожидания)
        if (!_seats.Wait(0))
        {
            OnCustomerRejected?.Invoke(customer);
            return false;
        }

        _queue.Enqueue(customer);
        Interlocked.Increment(ref _currentCount);
        OnCustomerEntered?.Invoke(customer);

        return true;
    }

    public bool TryGetCustomer(out Customer? customer)
    {
        if (_queue.TryDequeue(out customer))
        {
            Interlocked.Decrement(ref _currentCount);
            _seats.Release();
            OnCustomerLeft?.Invoke(customer);
            return true;
        }

        customer = null;
        return false;
    }

    public async Task<Customer?> WaitForCustomerAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (TryGetCustomer(out var customer))
            {
                return customer;
            }

            await Task.Delay(50, ct);
        }

        return null;
    }
    public IEnumerable<Customer> GetWaitingCustomers()
    {
        return _queue.ToArray();
    }

    public void Clear()
    {
        while (_queue.TryDequeue(out _))
        {
            Interlocked.Decrement(ref _currentCount);
            _seats.Release();
        }
    }
}