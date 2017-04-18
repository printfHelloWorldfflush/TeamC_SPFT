using SharePointFileTransfer_Library;
using System.Threading;
using System.Windows;

namespace SharePointFileTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            showMonitorPage();
        }

        private void showMonitorPage()
        {
            frame.Navigate(new MonitorPage());
        }
        private void showSettingPage()
        {
            frame.Navigate(new SettingPage());
        }
        private void showChangecredentialPage()
        {
            frame.Navigate(new ChangeCredentialPage());
        }
        private void MonitorItem_Click(object sender, RoutedEventArgs e)
        {
            showMonitorPage();
        }
        private void SettingItem_Click(object sender, RoutedEventArgs e)
        {
            showSettingPage();
        }
        private void ChangeCredential_Click(object sender, RoutedEventArgs e)
        {
            showChangecredentialPage();
        }
    }
}
