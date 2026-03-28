using System.Threading.Channels;

namespace CallCenter
{
    internal class Employee
    {
        private static int _workerCounter = 0;
        private static int _managerCounter = 0;

        private readonly bool _isManager = false;
        private string Name { get; }
        private ChannelReader<Call>? _reader;
        private Thread? _thread;
        private CancellationToken _cancellationToken;
        private WorkerAvailability? _availability;

        public Employee(bool isManager)
        {
            _isManager = isManager;
            Name = _isManager ? $"Manager{++_managerCounter}" : $"Worker{++_workerCounter}";
        }

        public Employee()
        {
            Name = $"Worker{++_workerCounter}";
        }

        public void Start(ChannelReader<Call> reader, CancellationToken cancellationToken = default, WorkerAvailability? availability = null)
        {
            _reader = reader;
            _cancellationToken = cancellationToken;
            _availability = availability;
            _thread = new Thread(TimeToWork) { Name = Name, IsBackground = true };
            _thread.Start();
        }

        public void Join()
        {
            _thread?.Join();
            //free resource ahead of GC
            _thread = null;
        }

        private void TimeToWork()
        {
            Console.WriteLine($"{Name} is waiting for calls...");
            try
            {
                if (_isManager) ManagerLoop();
                else WorkerLoop();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"{Name} stopped gracefully");
            }
        }

        private void WorkerLoop()
        {
            _availability?.SetIdle();
            while (_reader!.WaitToReadAsync(_cancellationToken).AsTask().GetAwaiter().GetResult())
            {
                if (_reader.TryRead(out var call))
                {
                    _availability?.SetBusy();
                    HandleCall(call);
                    _availability?.SetIdle();
                    Console.WriteLine($"{_reader?.Count} customer(s) left waiting");
                }
            }
        }

        private void ManagerLoop()
        {
            while (true)
            {
                _availability?.WaitUntilAllBusy(_cancellationToken);
                if (!_reader!.WaitToReadAsync(_cancellationToken).AsTask().GetAwaiter().GetResult())
                    break;
                if (_reader.TryRead(out var call))
                {
                    HandleCall(call);
                    Console.WriteLine($"{_reader?.Count} customer(s) left waiting");
                }
                   

            }
        }

        private void HandleCall(Call call)
        {
            Console.WriteLine($"{Name} handling call for {call.callTime} s");
            Thread.Sleep(call.callTime * 1000);
            Console.WriteLine($"{Name} finished handling call");
        }
    }
}
