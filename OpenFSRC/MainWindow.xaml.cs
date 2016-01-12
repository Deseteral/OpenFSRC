using System;
using System.Timers;
using System.Windows;

namespace OpenFSRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Simulation simulation;
        private static Timer updateTimer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, System.EventArgs e)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (simulation != null)
            {
                if (simulation.Connected)
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
            updateTimer = new Timer(500);
            updateTimer.Elapsed += updateTimer_Elapsed;
            updateTimer.Start();

            simulation = new FSXSimulation();
            simulation.Connect();

            UpdateStatus();
        }

        private void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            simulation.Update();
        }

        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (updateTimer != null)
                updateTimer.Stop();

            if (simulation != null)
                simulation.Disconnect();

            UpdateStatus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (simulation != null)
                simulation.Disconnect();
        }
    }
}
