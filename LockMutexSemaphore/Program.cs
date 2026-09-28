using LockMutexSemaphore.Synchronization;
using LockMutexSemaphore.Synchronization.Scenarios;

ISynchronizationScenario[] scenarios =
{
	new LockScenario(),
	new MutexScenario(),
	new SemaphoreScenario()
};

foreach (var scenario in scenarios)
{
	scenario.Run();
}
