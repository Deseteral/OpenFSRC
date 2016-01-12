namespace OpenFSRC.DataStructures
{
    enum Definitions
    {
        PositionInformation
    }

    enum Requests
    {
        PositionInformationRequest
    }

    public struct PositionInformation
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }

    public class SimulationData
    {
        public PositionInformation PositionInformation { get; set; }
    }
}