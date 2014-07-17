using System;
using System.Messaging;
using msmq = System.Messaging;
using System.Windows.Forms;
using Messages;

namespace TxClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnSend.Click += BtnSendOnClick;
        }

        private void BtnSendOnClick(object sender, EventArgs eventArgs)
        {
            if (_isDisposed) throw new ObjectDisposedException("Form1");

            // Send a request to the server
            using (var queue = new MessageQueue(@".\private$\TxServerQueue"))
            {
                using (var tx = new MessageQueueTransaction())
                using (var message = new msmq.Message())
                {
                    tx.Begin();
                    var request = new BasicMessage
                    {
                        Content = "Message sent from Basic Client at " + DateTime.Now.ToShortTimeString()
                    };
                    message.Body = request;

                    queue.Send(message, tx);
                    tx.Commit();
                }
            }
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
                _isDisposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
