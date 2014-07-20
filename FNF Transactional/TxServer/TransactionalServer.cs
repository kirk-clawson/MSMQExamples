using System;
using System.Messaging;
using System.Threading;
using Common;
using Messages;

namespace TxServer
{
    public class TransactionalServer : IWindowsService
    {
        private const string QueueAddress = @".\private$\TxServerQueue";
        private MessageQueue _txQueue;

        private static volatile bool _stopRequested;
        private static volatile bool _stopReady;

        public TransactionalServer()
        {
            ServiceName = "Transactional Server";
        }

        public string ServiceName { get; private set; }

        public void OnStart(string[] args)
        {
            _stopRequested = false;
            _stopReady = true;
            _txQueue = new MessageQueue(QueueAddress, true);
            _txQueue.Formatter = new XmlMessageFormatter(new[] { typeof(BasicMessage) });
            _txQueue.PeekCompleted += TxQueueOnPeekCompleted;
            Thread.Sleep(2000); // so I can see WPF bindings
            _txQueue.BeginPeek();
        }

        public void OnStop()
        {
            _stopRequested = true;
            while (!_stopReady)
            {
                Thread.Sleep(500);
            }
            _txQueue.PeekCompleted -= TxQueueOnPeekCompleted;
            Thread.Sleep(2000); // so I can see WPF bindings
            _txQueue.Dispose();
            _txQueue = null;
        }

        private void TxQueueOnPeekCompleted(object sender, PeekCompletedEventArgs peekCompletedEventArgs)
        {
            if (_stopRequested) return;
            _stopReady = false;
            var txQueue = (MessageQueue) sender;
            using (var tx = new MessageQueueTransaction())
            {
                try
                {
                    tx.Begin();
                    using (Message currentMessage = txQueue.Receive(tx))
                    {
                        Thread.Sleep(2500); // some process
                    }
                    tx.Commit();
                }
                catch (Exception)
                {
                    tx.Abort(); // puts message back on the queue
                }
            }
            _stopReady = true;
            if (!_stopRequested) txQueue.BeginPeek();
        }

        public void Dispose()
        {

        }
    }
}