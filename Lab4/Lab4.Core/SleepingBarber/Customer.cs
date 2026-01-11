namespace Lab4.Core.SleepingBarber;

public class Customer
{
    private static int _idCounter;

    public int Id { get; }
    public DateTime ArrivalTime { get; }
    public DateTime? ServiceStartTime { get; private set; }
    public DateTime? ServiceEndTime { get; private set; }
    public CustomerState State { get; private set; }

    public Customer()
    {
        Id = Interlocked.Increment(ref _idCounter);
        ArrivalTime = DateTime.Now;
        State = CustomerState.Arrived;
    }

    public void StartService()
    {
        ServiceStartTime = DateTime.Now;
        State = CustomerState.BeingServed;
    }

    public void EndService()
    {
        ServiceEndTime = DateTime.Now;
        State = CustomerState.Served;
    }

    public void Leave()
    {
        State = CustomerState.Left;
    }

    public TimeSpan? WaitTime => ServiceStartTime - ArrivalTime;
    public TimeSpan? ServiceDuration => ServiceEndTime - ServiceStartTime;

    public override string ToString() => $"Клиент #{Id}";

    public static void ResetCounter() => _idCounter = 0;
}

public enum CustomerState
{
    Arrived,
    Waiting,
    BeingServed,
    Served,
    Left
}