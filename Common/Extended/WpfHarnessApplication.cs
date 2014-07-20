using System;
using System.Windows;

namespace Common.Extended
{
    public static class WpfHarnessApplication
    {
        [STAThread]
        public static void Run(IWindowsService serviceToRun)
        {
            var app = new Application();
            var vm = new ServiceViewModel(serviceToRun);
            app.Run(new WpfHarnessWindow(vm));
        }
    }
}
