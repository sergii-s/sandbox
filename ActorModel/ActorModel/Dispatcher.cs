using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorModel
{
    public class Dispatcher
    {
        private readonly MessageListener _messageListener;
        private readonly int _workersCount;
        private Worker[] _workers;
        private ManualResetEvent[] _workerEvents;

        public Dispatcher(MessageListener messageListener, int workersCount)
        {
            _messageListener = messageListener;
            _workersCount = workersCount;
        }

        public void Start()
        {
            _workers = CreateWorkers().ToArray();
            _workerEvents = _workers.Select(x => x.FreeEvent).ToArray();
            Parallel.ForEach(_workers, worker => new Task(worker.Start).Start());
            new Task(GetMessage).Start();
        }

        public void GetMessage()
        {
            while (true)
            {
                var message = _messageListener.Pop();
                if (message == null)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                var worker = _workers.FirstOrDefault(x => x.SessionId == message.SessionId);
                if (worker != null)
                {
                    worker.AddJob(message);
                }
                else
                {
                    var workerId = WaitHandle.WaitAny(_workerEvents);
                    _workers[workerId].SessionId = message.SessionId;
                    _workers[workerId].AddJob(message);
                }
            }
        }

        private IEnumerable<Worker> CreateWorkers()
        {
            for (var i = 0; i < _workersCount; i++)
            {
                yield return new Worker(i);
            }
        }
    }
}
