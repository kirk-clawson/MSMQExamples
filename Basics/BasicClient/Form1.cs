using System;
using Messages;
using msmq = System.Messaging;
using System.Messaging;
using System.Windows.Forms;


namespace BasicClient
{
    public partial class Form1 : Form
    {
        // This is where we listen for the server's responses
        private MessageQueue _responseQueue = new MessageQueue(@".\private$\BasicResponseQueue");

        public Form1()
        {
            InitializeComponent();
            btnSend.Click += BtnSendOnClick;
            // we have to specify what type of incoming messages we're expecting when we use default XML
            _responseQueue.Formatter = new XmlMessageFormatter(new[] { typeof(BasicMessage) });
            _responseQueue.ReceiveCompleted += ResponseQueueOnReceiveCompleted;
            _responseQueue.BeginReceive();
        }

        private void AddServerResponse(string response)
        {
            // Stuff to keep WinForms happy when updating a UI control from a background thread
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddServerResponse), new object[] {response});
                return;
            }
            txtResponse.Text += response + Environment.NewLine;
        }

        private void BtnSendOnClick(object sender, EventArgs eventArgs)
        {
            if (_isDisposed) throw new ObjectDisposedException("Form1");

            // Send a request to the server
            using (var queue = new MessageQueue(@".\private$\BasicServerQueue"))
            {
                using (var message = new msmq.Message())
                {
                    // Tell the server where to send the response
                    message.ResponseQueue = _responseQueue;

                    var request = new BasicMessage
                    {
                        Content = "Message sent from Basic Client at " + DateTime.Now.ToShortTimeString()
                    };
                    message.Body = request;

                    queue.Send(message);
                }
            }
        }

        private void ResponseQueueOnReceiveCompleted(object sender, ReceiveCompletedEventArgs receiveCompletedEventArgs)
        {
            if (_isDisposed) throw new ObjectDisposedException("Form1");

            var queue = (MessageQueue) sender;
            
            // Read the message in the queue that caused this event to fire
            msmq.Message serverResponse = queue.EndReceive(receiveCompletedEventArgs.AsyncResult);
            
            // Note at this point, the message has been removed from the queue.
            // if you error out now, and don't finish processing it, the message is lost.
            var serverMessage = serverResponse.Body as BasicMessage;
            if (serverMessage != null)
            {
                AddServerResponse(serverMessage.Content);
            }

            // Wait for the next message...
            queue.BeginReceive();
        }

        #region IDisposable Implementation

        private bool _isDisposed;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (_responseQueue != null)
                {
                    _responseQueue.ReceiveCompleted -= ResponseQueueOnReceiveCompleted;
                    _responseQueue.Dispose();
                    _responseQueue = null;
                }

                _isDisposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
