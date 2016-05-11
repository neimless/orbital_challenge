using System.Collections.Generic;

namespace OrbitalC
{
    public class Route
    {
        public List<string> Satellites { get; set; }
        public double DistanceTravelled { get; set; }
        public double NumberOfHops { get { return Satellites.Count; } }

        public string GetRouteString()
        {
            return string.Join(",", Satellites);
        }
    }
}
