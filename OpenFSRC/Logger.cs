using System;

namespace OpenFSRC
{
    public abstract class Logger
    {
        public static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
