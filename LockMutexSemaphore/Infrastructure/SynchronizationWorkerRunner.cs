namespace LockMutexSemaphore.Infrastructure;

internal static class SynchronizationWorkerRunner
{
    internal static void RunWorkers(string name, int workerCount, Action<int, ManualResetEventSlim> work)
    {
        using var startGate = new ManualResetEventSlim(false);
        using var allWorkersReady = new CountdownEvent(workerCount);
        var threads = new Thread[workerCount];

        for (var index = 0; index < workerCount; index++)
        {
            var workerId = index + 1;
            threads[index] = new Thread(() =>
            {
                allWorkersReady.Signal();
                work(workerId, startGate);
            })
            {
                Name = $"{name}-worker-{workerId}"
            };
            threads[index].Start();
        }

        allWorkersReady.Wait();
        Console.WriteLine($"Releasing {workerCount} threads to contend together.");
        startGate.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    internal static void UpdateMaximum(ref int maximum, int value)
    {
        int currentMaximum;
        do
        {
            currentMaximum = maximum;
            if (value <= currentMaximum)
            {
                return;
            }
        } while (Interlocked.CompareExchange(ref maximum, value, currentMaximum) != currentMaximum);
    }
}