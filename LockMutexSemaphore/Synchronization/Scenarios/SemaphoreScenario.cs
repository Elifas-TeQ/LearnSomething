using LockMutexSemaphore.Infrastructure;

namespace LockMutexSemaphore.Synchronization.Scenarios;

public sealed class SemaphoreScenario : ISynchronizationScenario
{
    private const int WorkerCount = 5;
    private const int PermitCount = 2;

    public void Run()
    {
        Console.WriteLine($"=== SemaphoreSlim: up to {PermitCount} threads enter at a time ===");
        using var semaphore = new SemaphoreSlim(PermitCount, PermitCount);
        var activeThreads = 0;
        var maximumActiveThreads = 0;

        SynchronizationWorkerRunner.RunWorkers("semaphore", WorkerCount, (workerId, startGate) =>
        {
            startGate.Wait();
            Console.WriteLine($"Thread {workerId} is waiting for a permit.");
            semaphore.Wait();

            try
            {
                var active = Interlocked.Increment(ref activeThreads);
                SynchronizationWorkerRunner.UpdateMaximum(ref maximumActiveThreads, active);
                Console.WriteLine($"Thread {workerId} acquired a permit ({active} active).");
                Thread.Sleep(500);
                active = Interlocked.Decrement(ref activeThreads);
                Console.WriteLine($"Thread {workerId} releasing a permit ({active} remain active).");
            }
            finally
            {
                semaphore.Release();
            }
        });

        Console.WriteLine($"Maximum simultaneous threads using permits: {maximumActiveThreads} of {PermitCount}.");
        Console.WriteLine("SemaphoreSlim limits access by permit count, does not track thread ownership, and does not guarantee wait order.");
        Console.WriteLine();
    }
}