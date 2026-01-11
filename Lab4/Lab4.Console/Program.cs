using Lab4.Core.DiningPhilosophers;
using Lab4.Core.SleepingBarber;
using Lab4.Core.ProducerConsumer;

while (true)
{
    Console.WriteLine("\nВыберите задачу:");
    Console.WriteLine("1. Обедающие философы (с deadlock)");
    Console.WriteLine("2. Обедающие философы (безопасная версия)");
    Console.WriteLine("3. Спящий парикмахер");
    Console.WriteLine("4. Производитель-Потребитель");
    Console.WriteLine("0. Выход");
    Console.Write("\nВаш выбор: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await RunDiningPhilosophersDeadlock();
            break;
        case "2":
            await RunDiningPhilosophersSafe();
            break;
        case "3":
            await RunSleepingBarber();
            break;
        case "4":
            await RunProducerConsumer();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Неверный выбор!");
            break;
    }
}

async Task RunDiningPhilosophersDeadlock()
{
    Console.WriteLine("\n" + new string('=', 60));
    Console.WriteLine("ОБЕДАЮЩИЕ ФИЛОСОФЫ — ВЕРСИЯ С DEADLOCK");
    Console.WriteLine(new string('=', 60));
    Console.WriteLine("Нажмите Enter для запуска (Ctrl+C для прерывания)...");
    Console.ReadLine();

    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

    var table = new DiningTableDeadlock(5);
    table.StartWithDeadlock(cts.Token);

    // Периодически проверяем на deadlock
    var checkTask = Task.Run(async () =>
    {
        while (!cts.Token.IsCancellationRequested)
        {
            await Task.Delay(500);
            if (table.IsDeadlocked())
            {
                Console.WriteLine("\n!!! ОБНАРУЖЕН DEADLOCK !!!");
                Console.WriteLine("Все философы голодны, но никто не может есть.");
                cts.Cancel();
                break;
            }
        }
    });

    table.WaitAll(6000);
    await checkTask;

    Console.WriteLine("\n" + table.GetStatistics());
}

async Task RunDiningPhilosophersSafe()
{
    Console.WriteLine("\n" + new string('=', 60));
    Console.WriteLine("ОБЕДАЮЩИЕ ФИЛОСОФЫ — БЕЗОПАСНАЯ ВЕРСИЯ");
    Console.WriteLine(new string('=', 60));

    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

    var table = new DiningTableSafe(5);
    table.StartSafe(cts.Token);

    try
    {
        await table.WaitAllAsync();
    }
    catch (OperationCanceledException)
    {
        // Ожидаемое завершение
    }

    Console.WriteLine("\n" + table.GetStatistics());
}

async Task RunSleepingBarber()
{
    Console.WriteLine("\n" + new string('=', 60));
    Console.WriteLine("СПЯЩИЙ ПАРИКМАХЕР");
    Console.WriteLine(new string('=', 60));

    Customer.ResetCounter();
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

    var shop = new BarberShop(waitingRoomCapacity: 3);
    shop.Open(cts.Token);
    shop.StartCustomerGenerator(minIntervalMs: 200, maxIntervalMs: 800, cts.Token);

    try
    {
        await shop.CloseAsync();
    }
    catch (OperationCanceledException)
    {
        // Ожидаемое завершение
    }

    await Task.Delay(500); // Даём завершить текущую стрижку

    Console.WriteLine("\n" + shop.GetStatistics());
}

async Task RunProducerConsumer()
{
    Console.WriteLine("\n" + new string('=', 60));
    Console.WriteLine("ПРОИЗВОДИТЕЛЬ–ПОТРЕБИТЕЛЬ");
    Console.WriteLine(new string('=', 60));

    Product.ResetCounter();
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));

    var buffer = new BoundedBuffer<Product>(capacity: 5);

    var producer1 = new Producer("Завод-1", buffer);
    var producer2 = new Producer("Завод-2", buffer);
    var consumer1 = new Consumer("Магазин-1", buffer);
    var consumer2 = new Consumer("Магазин-2", buffer);

    Console.WriteLine("Запуск 2 производителей и 2 потребителей...\n");

    var tasks = new List<Task>
    {
        producer1.ProduceIndefinitelyAsync(100, 300, cts.Token),
        producer2.ProduceIndefinitelyAsync(150, 350, cts.Token),
        consumer1.ConsumeIndefinitelyAsync(200, 400, cts.Token),
        consumer2.ConsumeIndefinitelyAsync(250, 450, cts.Token)
    };

    try
    {
        await Task.WhenAll(tasks);
    }
    catch (OperationCanceledException)
    {
        // Ожидаемое завершение
    }

    Console.WriteLine($"""

        СТАТИСТИКА
        {producer1.Name}: произведено {producer1.ItemsProduced}
        {producer2.Name}: произведено {producer2.ItemsProduced}
        {consumer1.Name}: потреблено {consumer1.ItemsConsumed}
        {consumer2.Name}: потреблено {consumer2.ItemsConsumed}
        Осталось в буфере: {buffer.Count}
        """);
}