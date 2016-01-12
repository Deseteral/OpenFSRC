using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OpenFSRC
{
    class FSXSimulation : ISimulation
    {
        private SimConnect sim;

        private const int WM_USER_SIMCONNECT = 0x0402;

        private bool connected;
        public bool Connected
        {
            get
            {
                return connected;
            }

            private set
            {
                connected = value;
            }
        }

        public void Connect()
        {
            if (connected)
                return;

            connected = true;

            try
            {
                sim = new SimConnect(
                    "OpenFSRC",
                    new WindowInteropHelper(Application.Current.MainWindow).Handle,
                    WM_USER_SIMCONNECT,
                    null,
                    0
                );
            }
            catch (COMException ex)
            {
                connected = false;

                MessageBox.Show(
                    "A connection to the FSX could not be established. Make sure that FSX is running.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        public void Disconnect()
        {
            if (!connected)
                return;

            if (sim != null)
            {
                sim.Dispose();
                sim = null;
            }

            connected = false;
        }
    }
}
