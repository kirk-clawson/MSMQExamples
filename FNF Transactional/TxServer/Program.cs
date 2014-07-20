using System;
using System.ServiceProcess;
using Common;
using Common.Extended;

namespace TxServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var server = new TransactionalServer();

            if (Environment.UserInteractive)
            {
                WpfHarnessApplication.Run(server);
            }
            else
            {
                ServiceBase.Run(new SmcHarness(server));
            }
        }
    }
}
