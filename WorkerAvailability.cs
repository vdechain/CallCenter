using System.Threading;

namespace CallCenter
{
    internal class WorkerAvailability
    {
        private int _idleWorkers;
        private readonly ManualResetEventSlim _allBusy = new(false);

        public void SetIdle()
        {
            //Interlocked is atomic equivalent in java 
            //ref keyword is to pass the variable by reference. only needed for value types
            if (Interlocked.Increment(ref _idleWorkers) == 1)
                _allBusy.Reset(); // at least one worker is now free
        }

        public void SetBusy()
        {
            if (Interlocked.Decrement(ref _idleWorkers) == 0)
                _allBusy.Set(); // all workers are now busy
        }

        public void WaitUntilAllBusy(CancellationToken ct) => _allBusy.Wait(ct);
    }
}
