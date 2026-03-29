using System.Threading.Channels;

namespace CallCenter
{
    internal class CallCenter
    {
        // FIFO queue without size limit
        private Channel<Call> _channel = Channel.CreateUnbounded<Call>();
        private CancellationTokenSource _cts = new();
        private List<Employee> _employees;

        public CallCenter(int nbWorkers, int nbManagers) {
            _employees = [.. Enumerable.Range(0, nbWorkers).Select(_ => new Employee()),
                          .. Enumerable.Range(0, nbManagers).Select(_ => new Employee(true))];

            Console.WriteLine($"Call center with {nbManagers} manager(s) and {nbWorkers} workers initialized");
        }

        public async Task Start() => await TimeToWork();

        public void Stop() {
            _cts.Cancel();
            _channel.Writer.Complete();
        }

        internal async void AddCall(int v)
        {
            await _channel.Writer.WriteAsync(new Call(v));
            Console.WriteLine($"Incoming call ! {_channel.Reader.Count} customer(s) waiting");
        }

        private async Task TimeToWork() {
            Console.WriteLine("Day started !");
            var availability = new WorkerAvailability();
            await Task.WhenAll(_employees.Select(e => e.Start(_channel.Reader, _cts.Token, availability)));
            Console.WriteLine("Day ended !");
        }
    }
}
