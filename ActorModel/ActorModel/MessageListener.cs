using System;
using System.Collections.Generic;

namespace ActorModel
{
    public class MessageListener
    {
        private readonly Object _locker = new object();
        private readonly Queue<Message> _queue = new Queue<Message>();

        public void Push(Message message)
        {
            lock (_locker)
            {
                Console.WriteLine("Sent : " + message.MessageContent);
                _queue.Enqueue(message);
            }
        }

        public Message Pop()
        {
            lock (_locker)
            {
                return _queue.Dequeue();
            }
        }
    }
}