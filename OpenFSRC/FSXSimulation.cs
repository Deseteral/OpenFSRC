using Microsoft.FlightSimulator.SimConnect;
using OpenFSRC.DataStructures;
using System;
using System.Windows;
using System.Windows.Interop;

namespace OpenFSRC
{
    public class FSXSimulation : Simulation
    {
        private SimConnect sim;
        private const int WM_USER_SIMCONNECT = 0x0402;
        
        public override void Connect()
        {
            if (Connected)
                return;

            Connected = true;

            try
            {
                WindowInteropHelper wih = new WindowInteropHelper(Application.Current.MainWindow);

                sim = new SimConnect(
                    "OpenFSRC",
                    wih.Handle,
                    WM_USER_SIMCONNECT,
                    null,
                    0
                );
                
                HwndSource hs = HwndSource.FromHwnd(wih.Handle);
                hs.AddHook(new HwndSourceHook(DefWndProcWpf));

                InitializeApi();
            }
            catch (Exception ex)
            {
                Connected = false;

                MessageBox.Show(
                    "A connection to the FSX could not be established. Make sure that FSX is running.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        public override void Disconnect()
        {
            if (!Connected)
                return;

            if (sim != null)
            {
                sim.Dispose();
                sim = null;
            }

            Connected = false;
        }

        public override void Update()
        {
            if (!Connected)
                return;

            // PositionInformation
            sim.RequestDataOnSimObjectType(
                Requests.PositionInformationRequest,
                Definitions.PositionInformation,
                0,
                SIMCONNECT_SIMOBJECT_TYPE.USER
            );
        }

        private void DataReceived(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            switch ((Requests)data.dwRequestID)
            {
                case Requests.PositionInformationRequest:
                    Data.PositionInformation = (PositionInformation)data.dwData[0];
                    break;

                default:
                    Console.WriteLine("Unknown request ID: " + data.dwRequestID);
                    break;
            }
        }

        private void InitializeApi()
        {
            // Define a data structures

            // PositionInformation
            sim.AddToDataDefinition(Definitions.PositionInformation, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            sim.AddToDataDefinition(Definitions.PositionInformation, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            sim.AddToDataDefinition(Definitions.PositionInformation, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            sim.RegisterDataDefineStruct<PositionInformation>(Definitions.PositionInformation);

            // Data received callback
            sim.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(DataReceived);
        }

        private IntPtr DefWndProcWpf(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;

            if (msg == WM_USER_SIMCONNECT)
            {
                if (Connected && sim != null)
                {
                    sim.ReceiveMessage();
                    handled = true;
                }
            }

            return (IntPtr)0;
        }
    }
}
