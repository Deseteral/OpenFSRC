using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Threading;
using System.Windows;

namespace OpenFSRC.Networking
{
    public class HttpServer : NancyModule
    {
        private Thread hostThread;
        
        public HttpServer()
        {
            Get["/"] = _ => "OpenFSRC";
        }

        public void Start()
        {
            Stop();

            hostThread = new Thread(new ThreadStart(ThreadBegin));
            hostThread.Start();
        }

        public void Stop()
        {
            if (hostThread != null)
                hostThread.Abort();
        }

        private void ThreadBegin()
        {
            HostConfiguration config = new HostConfiguration();
            config.UrlReservations.CreateAutomatically = true;

            // TODO: Allow user to change the port
            using (var host = new NancyHost(config, new Uri("http://localhost:8957")))
            {
                try
                {
                    host.Start();

                    // Keep the server thread alive
                    while (true);
                }
                catch (AutomaticUrlReservationCreationFailureException ex)
                {
                    MessageBox.Show(
                        "Couldn't start HTTP server on this port",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }
    }
}
