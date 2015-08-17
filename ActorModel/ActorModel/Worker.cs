using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ActorModel
{
    public class Worker
    {
        private readonly ConcurrentQueue<Message> _jobQueue = new ConcurrentQueue<Message>();
        public ManualResetEvent FreeEvent { get; set; }
        private ManualResetEvent JobReadyEvent { get; set; }
        private readonly int _workerId;

        public Worker(int workerId)
        {
            _workerId = workerId;
            FreeEvent = new ManualResetEvent(true);
            JobReadyEvent = new ManualResetEvent(false);
        }

        public bool Busy { get; set; }
        public string SessionId { get; set; }

        public void AddJob(Message message)
        {
            FreeEvent.Reset();
            _jobQueue.Enqueue(message);
            JobReadyEvent.Set();
        }

        public void Start()
        {
            while (true)
            {
                Message job;
                if (!_jobQueue.TryDequeue(out job))
                {
                    FreeEvent.Set();
                    JobReadyEvent.WaitOne();
                    continue;
                }
                Console.WriteLine(_workerId + " " + job.MessageContent);
                Thread.Sleep(1000);
            }
        }
    }
}