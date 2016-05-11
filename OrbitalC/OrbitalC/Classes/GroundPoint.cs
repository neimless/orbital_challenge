using System.Collections.Generic;

namespace OrbitalC
{
    public class GroundPoint
    {
        public GroundPoint(double lat, double lon)
        {
            this.VisibleSatellites = new List<Satellite>();
            this.Latitude = lat;
            this.Longitude = lon;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<Satellite> VisibleSatellites { get; set; }
    }
}
