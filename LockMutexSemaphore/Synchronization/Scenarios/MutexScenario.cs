using LockMutexSemaphore.Infrastructure;

namespace LockMutexSemaphore.Synchronization.Scenarios;

public sealed class MutexScenario : ISynchronizationScenario
{
    private const int WorkerCount = 5;

    public void Run()
    {
        Console.WriteLine("=== Mutex: one thread owns the mutex at a time ===");
        using var mutex = new Mutex();
        var activeThreads = 0;
        var maximumActiveThreads = 0;

        SynchronizationWorkerRunner.RunWorkers("mutex", WorkerCount, (workerId, startGate) =>
        {
            startGate.Wait();
            Console.WriteLine($"Thread {workerId} is waiting for mutex.");
            mutex.WaitOne();

            try
            {
                activeThreads++;
                maximumActiveThreads = Math.Max(maximumActiveThreads, activeThreads);
                Console.WriteLine($"Thread {workerId} acquired mutex ({activeThreads} active).");
                Thread.Sleep(250);
                activeThreads--;
                Console.WriteLine($"Thread {workerId} releasing mutex.");
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        });

        Console.WriteLine($"Maximum simultaneous threads owning mutex: {maximumActiveThreads}.");
        Console.WriteLine("Mutex ownership is thread-affine and can be named for cross-process use; wait order is not guaranteed.");
        Console.WriteLine();
    }
}