using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace TxServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var server = new TransactionalServer();

            if (Environment.UserInteractive)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new WinFormHarness(server));
            }
            else
            {
                ServiceBase.Run(new ServiceHarness(server));
            }
        }
    }
}
