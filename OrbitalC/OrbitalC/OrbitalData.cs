using System;
using System.Collections.Generic;
using System.Linq;

namespace OrbitalC
{
    public class OrbitalData
    {
        public OrbitalData()
        {
            this.Satellites = new List<Satellite>();
            this.Routes = new List<string>();
        }

        public GroundPoint StartingPoint { get; set; }
        public GroundPoint EndingPoint { get; set; }
        public string Seed { get; set; }
        public List<Satellite> Satellites { get; set; }
        public List<string> Routes { get; set; }

        public void AddSatellite(string name, double lat, double lon, double h)
        {
            this.Satellites.Add(new Satellite { Name = name, Longitude = lon, Latitude = lat, Altitude = h, VisibleSatellites = new List<VisibleSatellite>() });
        }

        public void CheckSatellitePositions()
        {
            foreach (var sat in this.Satellites)
            {
                double distance;
                if (sat.IsVisibleToGroundPoint(this.StartingPoint.Latitude, this.StartingPoint.Longitude, out distance))
                {
                    this.StartingPoint.VisibleSatellites.Add(new VisibleSatellite { Satellite = sat, Distance = distance });
                }
                if (sat.IsVisibleToGroundPoint(this.EndingPoint.Latitude, this.EndingPoint.Longitude, out distance))
                {
                    this.EndingPoint.VisibleSatellites.Add(new VisibleSatellite { Satellite = sat, Distance = distance });
                }

                foreach (var item in this.Satellites)
                {
                    sat.CheckSatelliteVisibility(item);
                }                
            }
        }

        public void SearchRoute(List<VisibleSatellite> satellites, string link, List<Satellite> ignoreList)
        {
            foreach (var sat in satellites)
            {
                if (ignoreList.Select(x => x.Name).Contains(sat.Satellite.Name))
                {
                    continue;
                }
                ignoreList.Add(sat.Satellite);
                if (this.EndingPoint.VisibleSatellites.Select(x => x.Satellite.Name).Contains(sat.Satellite.Name))
                {
                    Routes.Add(link + "," + sat.Satellite.Name);
                    return;
                }
                SearchRoute(sat.Satellite.VisibleSatellites, link + "," + sat.Satellite.Name, ignoreList);
            }
        }
    }

    public class VisibleSatellite
    {
        public Satellite Satellite { get; set; }
        public double Distance { get; set; }
    }

    public class GroundPoint
    {
        public GroundPoint(double lat, double lon)
        {
            this.VisibleSatellites = new List<VisibleSatellite>();
            this.Latitude = lat;
            this.Longitude = lon;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<VisibleSatellite> VisibleSatellites { get; set; }
    }

    public class Satellite
    {
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }
        public List<VisibleSatellite> VisibleSatellites { get; set; }

        public void CheckSatelliteVisibility(Satellite sat)
        {
            if (sat.Name == this.Name)
                return;

            double distance;
            if (this.IsSatelliteVisible(sat.Latitude, sat.Longitude, sat.Altitude, out distance))
            {
                this.VisibleSatellites.Add(new VisibleSatellite { Satellite = sat, Distance = distance });
            }
        }

        public bool IsVisibleToGroundPoint(double lat, double lon, out double distance)
        {
            double earthRad = 6371;
            double angle = (180 / Math.PI) * Math.Acos(earthRad / (earthRad + this.Altitude));
            double coverageRadius = (angle / 360) * (2 * Math.PI * earthRad);
            distance = Calculate.GroundDistanceBetweenPoints(lat, this.Latitude, lon, this.Longitude);
            return distance < coverageRadius;
        }

        public bool IsSatelliteVisible(double lat, double lon, double alt, out double distance)
        {
            double earthRad = 6371;

            //Distance between two satellites using ground routes
            double groundDistance = Calculate.GroundDistanceBetweenPoints(lat, this.Latitude, lon, this.Longitude);
            double segmentAngle = 360 * (groundDistance / (2 * Math.PI * earthRad));
            segmentAngle = segmentAngle * (Math.PI / 180);

            //Direct distance of two satellites
            double satelliteDistance = Math.Sqrt(Math.Pow(earthRad + alt, 2) + Math.Pow(earthRad + this.Altitude, 2) - (2 * (earthRad+alt) * (earthRad+this.Altitude) * Math.Cos(segmentAngle)));
            distance = satelliteDistance;

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
