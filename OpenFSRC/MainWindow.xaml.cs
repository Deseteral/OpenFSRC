using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;

namespace OpenFSRC
{
    public partial class MainWindow : Window
    {
        private static Simulation simulation;
        private static Timer updateTimer;
        private static HttpServer http;

        private static WebSocketServer webSocketServer;
        private static List<IWebSocketConnection> webSockets;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, System.EventArgs e)
        {
            http = new HttpServer();

            // TODO: Allow user to change the port
            webSocketServer = new WebSocketServer("ws://0.0.0.0:8958");
            webSockets = new List<IWebSocketConnection>();

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

        private void Shutdown()
        {
            Logger.Log("Shutting down all systems");

            if (updateTimer != null)
                updateTimer.Stop();

            http.Stop();

            if (simulation != null)
                simulation.Disconnect();

            Logger.Log("Stop.");
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log("Connecting to FSX...");
            simulation = new FSXSimulation();
            simulation.Connect();

            if (!simulation.Connected)
                return;

            Logger.Log("Successfully connected to FSX...");

            // TODO: Allow user to change the interval
            updateTimer = new Timer(500);
            updateTimer.Elapsed += updateTimer_Elapsed;
            updateTimer.Start();

            Logger.Log("Staring HTTP server...");
            http.Start();

            Logger.Log("Staring WebSockets server...");
            webSocketServer.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    webSockets.Add(socket);
                    Logger.Log("WebSocket connected!");
                };

                socket.OnClose = () =>
                {
                    webSockets.Remove(socket);
                    Logger.Log("WebSocket closed!");
                };
            });

            UpdateStatus();
        }

        private void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            simulation.Update();

            string json = JsonConvert.SerializeObject(simulation.Data);
            foreach (IWebSocketConnection socket in webSockets)
            {
                socket.Send(json);
            }
        }

        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
            UpdateStatus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Shutdown();
        }
    }
}
