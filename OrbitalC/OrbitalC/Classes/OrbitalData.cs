using System.Collections.Generic;
using System.Linq;

namespace OrbitalC
{
    public class OrbitalData
    {
        public OrbitalData()
        {
            this.Satellites = new List<Satellite>();
            this.Routes = new List<Route>();
        }

        public GroundPoint StartingPoint { get; set; }
        public GroundPoint EndingPoint { get; set; }
        public string Seed { get; set; }
        public List<Satellite> Satellites { get; set; }
        public List<Route> Routes { get; set; }

        public void AddSatellite(string name, double lat, double lon, double h)
        {
            this.Satellites.Add(new Satellite { Name = name, Longitude = lon, Latitude = lat, Altitude = h, VisibleSatellites = new List<Satellite>() });
        }

        public void CheckSatellitePositions()
        {
            foreach (var sat in this.Satellites)
            {
                if (sat.IsVisibleToGroundPoint(this.StartingPoint.Latitude, this.StartingPoint.Longitude))
                    this.StartingPoint.VisibleSatellites.Add(sat);
                if (sat.IsVisibleToGroundPoint(this.EndingPoint.Latitude, this.EndingPoint.Longitude))
                    this.EndingPoint.VisibleSatellites.Add(sat);

                foreach (var item in this.Satellites)
                {
                    sat.CheckSatelliteVisibility(item);
                }                
            }
        }
        
        public void SearchRoute(List<Satellite> visibleSatellites, List<Satellite> currentRoute)
        {
            foreach (var sat in visibleSatellites)
            {
                if (currentRoute.Any(x => x.Name == sat.Name))
                    continue;

                currentRoute.Add(sat);
                if (this.EndingPoint.VisibleSatellites.Any(x => x.Name == sat.Name))
                {
                    Routes.Add(new Route { Satellites = currentRoute.Select(x => x.Name).ToList() });
                    currentRoute.Remove(sat);
                    continue;
                }
                SearchRoute(sat.VisibleSatellites, currentRoute);
                currentRoute.Remove(sat);
            }
        }

        public void CalculateRouteDistances()
        {
            foreach (var route in this.Routes)
            {                
                if (route.Satellites == null || route.Satellites.Count == 0)
                    continue;
                var firstSatellite = GetSatelliteByName(route.Satellites.First());
                var lastSatellite = GetSatelliteByName(route.Satellites.Last());
                var startingPointDistance = Calculate.DistanceFromGroundToSatellite(this.StartingPoint.Latitude, firstSatellite.Latitude, this.StartingPoint.Longitude, firstSatellite.Longitude, firstSatellite.Altitude);
                var endingPointDistance = Calculate.DistanceFromGroundToSatellite(this.EndingPoint.Latitude, lastSatellite.Latitude, this.EndingPoint.Longitude, lastSatellite.Longitude, lastSatellite.Altitude);
                route.DistanceTravelled = startingPointDistance + endingPointDistance;
                if (route.Satellites.Count > 1)
                {
                    for (int i = 0; i < route.Satellites.Count - 1; i++)
                    {
                        var satelliteA = GetSatelliteByName(route.Satellites[i]);
                        var satelliteB = GetSatelliteByName(route.Satellites[i+1]);
                        var distance = Calculate.StraightDistanceBetweenSatellites(satelliteA.Latitude, satelliteB.Latitude, satelliteA.Longitude, satelliteB.Longitude, satelliteA.Altitude, satelliteB.Altitude);
                        route.DistanceTravelled += distance;
                    }
                }             
            }
        }

        public List<Route> GetRoutesWithLeastAmountOfHops()
        {
            var ordered = this.Routes.OrderBy(x => x.NumberOfHops);
            return ordered.Where(x => x.NumberOfHops == ordered.First().NumberOfHops).ToList();
        }

        public Route GetRouteWithShortestDistance()
        {
            return this.Routes.OrderBy(x => x.DistanceTravelled).First();
        }

        private Satellite GetSatelliteByName(string name)
        {
            return this.Satellites.FirstOrDefault(x => x.Name == name);
        }
    }  
}
