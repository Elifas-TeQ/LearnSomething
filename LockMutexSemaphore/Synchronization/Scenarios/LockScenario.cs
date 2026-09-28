using LockMutexSemaphore.Infrastructure;

namespace LockMutexSemaphore.Synchronization.Scenarios;

public sealed class LockScenario : ISynchronizationScenario
{
    private const int WorkerCount = 5;

    public void Run()
    {
        Console.WriteLine("=== lock: one thread enters the protected section at a time ===");
        var lockObject = new object();
        var activeThreads = 0;
        var maximumActiveThreads = 0;

        SynchronizationWorkerRunner.RunWorkers("lock", WorkerCount, (workerId, startGate) =>
        {
            startGate.Wait();
            Console.WriteLine($"Thread {workerId} is waiting for lock.");

            lock (lockObject)
            {
                activeThreads++;
                maximumActiveThreads = Math.Max(maximumActiveThreads, activeThreads);
                Console.WriteLine($"Thread {workerId} entered lock ({activeThreads} active).");
                Thread.Sleep(250);
                activeThreads--;
                Console.WriteLine($"Thread {workerId} leaving lock.");
            }
        });

        Console.WriteLine($"Maximum simultaneous threads inside lock: {maximumActiveThreads}.");
        Console.WriteLine("lock is process-local and thread-affine; waiting-thread order is not guaranteed.");
        Console.WriteLine();
    }
}