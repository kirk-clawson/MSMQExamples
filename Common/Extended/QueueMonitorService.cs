using System;
using System.Messaging;

namespace Common
{
    public abstract class QueueMonitorService : IWindowsService
    {
        private bool _isDisposed;
        private readonly object _threadLock = new object();
        private MessageQueue _monitoredQueue;

        private bool _stopRequested;

        protected QueueMonitorService(string queueAddress)
        {
            _monitoredQueue = new MessageQueue(queueAddress);
        }

        public abstract string ServiceName { get; protected set; }
        protected abstract IMessageFormatter GetQueueMessageFormatter();

        public void OnStart(string[] args)
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().Name);

            _stopRequested = false;
            _monitoredQueue.Formatter = GetQueueMessageFormatter();
            _monitoredQueue.PeekCompleted += MonitoredQueueOnPeekCompleted;
            _monitoredQueue.BeginPeek();
        }

        public void OnStop()
        {
            _stopRequested = true;
            lock (_threadLock)
            {
                _monitoredQueue.Close();
                _monitoredQueue.PeekCompleted -= MonitoredQueueOnPeekCompleted;
            }
        }

        private void MonitoredQueueOnPeekCompleted(object sender, PeekCompletedEventArgs peekCompletedEventArgs)
        {
            lock (_threadLock)
            {
                if (_stopRequested) return;
                using (var tx = new MessageQueueTransaction())
                {
                    tx.Begin();
                    try
                    {
                        using (Message message = _monitoredQueue.Receive(tx))
                        {
                            if (message != null)
                            {
                                ProcessMessage(tx, message);
                            }
                        }
                        tx.Commit();
                    }
                    catch (Exception)
                    {
                        tx.Abort();
                    }
                }
                if (!_stopRequested) _monitoredQueue.BeginPeek();
            }
        }

        /// <summary>
        /// Process the content contained in the MSMQ message.
        /// </summary>
        /// <param name="tx">
        /// The MSMQ transaction that the current message was pulled under. It is passed in to 
        /// allow multiple Message Queue reads or writes to be tied together under one transaction.
        /// </param>
        /// <param name="message">The message to process</param>
        /// <remarks>
        /// Note to Implementors: You must not explicitly call Commit() or Abort() on <paramref name="tx"/>.
        /// If you want the transaction to commit, simply exit the method normally. If you want the
        /// transaction to abort, throw an exception. 
        /// </remarks>
        protected abstract void ProcessMessage(MessageQueueTransaction tx, Message message);

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_monitoredQueue != null)
                {
                    _monitoredQueue.Dispose();
                    _monitoredQueue = null;
                }
            }
            _isDisposed = true;
        }

        #endregion

    }
}
