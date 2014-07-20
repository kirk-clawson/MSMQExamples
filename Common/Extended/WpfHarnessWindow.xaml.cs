using System.Windows;
using Common.Extended;

namespace Common
{
    /// <summary>
    /// Interaction logic for WPFServiceHarness.xaml
    /// </summary>
    internal partial class WpfHarnessWindow : Window
    {
        public WpfHarnessWindow(ServiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            ((ServiceViewModel)DataContext).StartService();
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            ((ServiceViewModel)DataContext).StopService();
        }
    }
}
