using System;
using System.Collections.Generic;

namespace OrbitalC
{
    public class Satellite
    {
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }
        public List<Satellite> VisibleSatellites { get; set; }

        public void CheckSatelliteVisibility(Satellite sat)
        {
            if (sat.Name == this.Name)
                return;

            if (this.IsSatelliteVisible(sat.Latitude, sat.Longitude, sat.Altitude))
            {
                this.VisibleSatellites.Add(sat);
            }
        }

        public bool IsVisibleToGroundPoint(double lat, double lon)
        {
            double earthRad = 6371;
            double angle = (180 / Math.PI) * Math.Acos(earthRad / (earthRad + this.Altitude));
            double coverageRadius = (angle / 360) * (2 * Math.PI * earthRad);
            double distance = Calculate.GroundDistanceBetweenPoints(lat, this.Latitude, lon, this.Longitude);
            return distance < coverageRadius;
        }

        public bool IsSatelliteVisible(double lat, double lon, double alt)
        {
            double earthRad = 6371;

            //Distance between two satellites using ground routes
            double groundDistance = Calculate.GroundDistanceBetweenPoints(lat, this.Latitude, lon, this.Longitude);

            // Segment angle from lines drawn to satellites from earth center
            double segmentAngle = 360 * (groundDistance / (2 * Math.PI * earthRad));
            segmentAngle = segmentAngle * (Math.PI / 180);

            double satelliteDistance = Calculate.StraightDistanceBetweenSatellites(lat, this.Latitude, lon, this.Longitude, alt, this.Altitude);

            //Distance from satellite to tangent
            double satelliteLimitDistance = Math.Sqrt(Math.Pow(this.Altitude + earthRad, 2) / Math.Pow(earthRad, 2));
            if (satelliteDistance < satelliteLimitDistance)
                return true;

            //Angle between center-of-earth-to-satellite and sat-sat line
            double satelliteAngle = (180 / Math.PI) * Math.Asin((earthRad + alt) * (Math.Sin(segmentAngle) / satelliteDistance));

            //Angle between center-of-earth-to-satellite and center-of-earth-to-tanget line
            double earthAngle = (180 / Math.PI) * Math.Asin(earthRad / (earthRad + this.Altitude));

            return Math.Abs(satelliteAngle) > Math.Abs(earthAngle);
        }
    }
}
