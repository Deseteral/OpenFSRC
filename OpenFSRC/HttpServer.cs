using Nancy;
using Nancy.Hosting.Self;
using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace OpenFSRC
{
    public class HttpServer : NancyModule
    {
        private Thread hostThread;

        private static string indexSource;

        public HttpServer()
        {
            // Read _index.html file
            try
            {
                string path = Directory.GetCurrentDirectory() + "/Content/_index.html";
                using (StreamReader sr = new StreamReader(path))
                {
                    indexSource = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Logger.Log("_index.html could not be read:");
                Logger.Log(e.Message);
            }

            Get["/"] = _ => {
                return indexSource;
            };
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
                    Logger.Log("Couldn't start HTTP server on this port");

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
