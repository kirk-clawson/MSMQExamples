using System;
using System.Messaging;
using System.Threading;
using Messages;

namespace BasicServer
{
    class Server : IDisposable
    {
        private MessageQueue _serverQueue = new MessageQueue(@".\private$\BasicServerQueue");

        public Server()
        {
            // we have to specify what type of incoming messages we're expecting when we use default XML
            _serverQueue.Formatter = new XmlMessageFormatter(new[] { typeof(BasicMessage) });
            _serverQueue.ReceiveCompleted += ServerQueueOnReceiveCompleted;
            _serverQueue.BeginReceive();
        }

        private void ServerQueueOnReceiveCompleted(object sender, ReceiveCompletedEventArgs receiveCompletedEventArgs)
        {
            var queue = (MessageQueue) sender;

            using (Message clientRequest = queue.EndReceive(receiveCompletedEventArgs.AsyncResult))
            {
                var request = clientRequest.Body as BasicMessage;

                if (request != null)
                {
                    Console.WriteLine("Message Received from client:");
                    Console.WriteLine(" - " + request.Content);
                    Console.WriteLine("Sending response in one moment...");
                    Thread.Sleep(1000); // Simulate some long process...

                    using (MessageQueue responseQueue = clientRequest.ResponseQueue) // Note we don't need to know the address.
                    {
                        var response = new BasicMessage
                        {
                            Content = "Server response on " + DateTime.Now.ToShortTimeString()
                        };
                        responseQueue.Send(response);
                    }
                }
            }

            queue.BeginReceive();
        }

        public void Dispose()
        {
            if (_serverQueue != null)
            {
                _serverQueue.ReceiveCompleted -= ServerQueueOnReceiveCompleted;
                _serverQueue.Dispose();
                _serverQueue = null;
            }
        }
    }
}
