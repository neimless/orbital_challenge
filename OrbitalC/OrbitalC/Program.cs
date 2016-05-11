using System;
using System.Collections.Generic;
using System.Linq;

namespace OrbitalC
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataApi = new DataApi();
            var data = dataApi.GetInitialData();
            data.CheckSatellitePositions();
            data.SearchRoute(data.StartingPoint.VisibleSatellites, new List<Satellite>());
            data.CalculateRouteDistances();

            Console.WriteLine(data.Seed);
            Console.WriteLine("Routes with least hops: ");
            foreach (var route in data.GetRoutesWithLeastAmountOfHops())
            {
                Console.WriteLine(route.GetRouteString());
            }
            Console.WriteLine("Shortest route: ");
            var shortest = data.GetRouteWithShortestDistance();
            Console.WriteLine(shortest.GetRouteString() + "   :   " + shortest.DistanceTravelled.ToString());

            WriteDebugData(data);
            
            Console.ReadLine();
        }
        
        static void WriteDebugData(OrbitalData data)
        {
            Console.WriteLine();
            Console.WriteLine(data.StartingPoint.Latitude + "," + data.StartingPoint.Longitude + "," + data.EndingPoint.Latitude + "," + data.EndingPoint.Longitude);
            foreach (var sat in data.Satellites)
            {
                Console.WriteLine(sat.Name + "," + sat.Latitude + "," + sat.Longitude + "," + sat.Altitude);
            }                      

            Console.WriteLine("Start: ");
            foreach (var sat in data.StartingPoint.VisibleSatellites)
            {
                Console.WriteLine(sat.Name);
            }

            Console.WriteLine("End: ");
            foreach (var sat in data.EndingPoint.VisibleSatellites)
            {
                Console.WriteLine(sat.Name);
            }
            Console.WriteLine();

            Console.WriteLine("SatelliteData: ");
            foreach (var sat in data.Satellites)
            {
                Console.WriteLine(sat.Name);
                sat.VisibleSatellites.Select(x => x.Name).ToList().ForEach(x => Console.Write(x + " "));
                Console.WriteLine();
            }

            Console.WriteLine("Routes: ");
            foreach (var route in data.Routes.OrderBy(x => x.NumberOfHops))
            {
                Console.WriteLine(route.GetRouteString() + "  :  "  + route.DistanceTravelled.ToString());
            }
        }        
    }
}
