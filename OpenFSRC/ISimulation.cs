using System;

namespace OpenFSRC
{
    interface ISimulation
    {
        bool Connected { get; }

        void Connect();
        void Disconnect();
    }
}
