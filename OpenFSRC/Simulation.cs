using OpenFSRC.DataStructures;

namespace OpenFSRC
{
    public abstract class Simulation
    {
        public bool Connected { get; protected set; }
        public SimulationData Data { get; protected set; }

        public Simulation()
        {
            Data = new SimulationData();
        }

        public abstract void Connect();
        public abstract void Disconnect();

        public abstract void Update();
    }
}
