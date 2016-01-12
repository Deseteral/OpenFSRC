using System.Windows;

namespace OpenFSRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ISimulation simulator;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateStatus()
        {
            if (simulator != null)
            {
                if (simulator.Connected)
                {
                    buttonConnect.IsEnabled = false;
                    buttonDisconnect.IsEnabled = true;
                }
                else
                {
                    buttonConnect.IsEnabled = true;
                    buttonDisconnect.IsEnabled = false;
                }
            }
            else
            {
                buttonConnect.IsEnabled = true;
                buttonDisconnect.IsEnabled = false;
            }
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            simulator = new FSXSimulation();
            simulator.Connect();

            UpdateStatus();
        }

        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (simulator != null)
                simulator.Disconnect();

            UpdateStatus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (simulator != null)
                simulator.Disconnect();
        }

        private void Window_Initialized(object sender, System.EventArgs e)
        {
            UpdateStatus();
        }
    }
}
