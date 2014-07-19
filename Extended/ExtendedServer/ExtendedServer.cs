using System.Messaging;
using Common;
using Messages;

namespace ExtendedServer
{
    public class ExtendedServer : QueueMonitorService
    {
        private const string QueueAddress = @".\private$\TxServerQueue";

        public ExtendedServer() : base(QueueAddress)
        {
        }

        public override string ServiceName
        {
            get { return "Extended Server"; }
        }

        protected override IMessageFormatter GetQueueMessageFormatter()
        {
            return new XmlMessageFormatter(new[] {typeof (BasicMessage)});
        }

        protected override void ProcessMessage(MessageQueueTransaction tx, Message message)
        {
            
        }
    }
}