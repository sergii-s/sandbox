using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace ActorModel
{
    public class Test
    {
        readonly Random _rm = new Random((int)DateTime.Now.Ticks);

        [Test]
        public void FactMethodName()
        {
            const int sessionsCount = 4;
            const int messagesCount = 200;

            var messageListener = new MessageListener();


            for (var i = 0; i < messagesCount; i++)
            {
                var session = _rm.Next(sessionsCount);
                messageListener.Push(new Message
                {
                    SessionId = session.ToString(CultureInfo.InvariantCulture),
                    MessageId = i,
                    MessageContent = String.Format("Session {0}, Message {1}", session, i)
                });
            }

            var dispatcher = new Dispatcher(messageListener, 4);
            dispatcher.Start();

            Thread.Sleep(100000);
        }
    }
}