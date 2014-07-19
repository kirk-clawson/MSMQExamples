using System.Messaging;
using Common;
using Messages;

namespace ExtendedServer
{
    public sealed class ExtendedServer : QueueMonitorService
    {
        private const string QueueAddress = @".\private$\TxServerQueue";

        public ExtendedServer() : base(QueueAddress)
        {
            ServiceName = "Extended Server";
        }

        public override string ServiceName { get; protected set; }
        
        protected override IMessageFormatter GetQueueMessageFormatter()
        {
            return new XmlMessageFormatter(new[] {typeof (BasicMessage)});
        }

        protected override void ProcessMessage(MessageQueueTransaction tx, Message message)
        {
            
        }
    }
}