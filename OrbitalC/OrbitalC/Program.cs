using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbitalC
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataApi = new DataApi();
            var data = dataApi.GetInitialData();

            Console.WriteLine(data.Seed);
            Console.WriteLine(data.StartingPoint.Latitude + "," + data.StartingPoint.Longitude + "," + data.EndingPoint.Latitude + "," + data.EndingPoint.Longitude);
            foreach (var sat in data.Satellites)
            {
                Console.WriteLine(sat.Name + "," + sat.Latitude + "," + sat.Longitude + "," + sat.Altitude);
            }

            data.CheckSatellitePositions();
            data.SearchRoute(data.StartingPoint.VisibleSatellites, string.Empty, new List<Satellite>());

            Console.WriteLine("Start: ");
            foreach (var sat in data.StartingPoint.VisibleSatellites)
            {
                Console.WriteLine(sat.Satellite.Name + "  :  " + sat.Distance.ToString());
            }

            Console.WriteLine("End: ");
            foreach (var sat in data.EndingPoint.VisibleSatellites)
            {
                Console.WriteLine(sat.Satellite.Name + "  :  " + sat.Distance.ToString());
            }
            Console.WriteLine();

            Console.WriteLine("SatelliteData: ");
            foreach (var sat in data.Satellites)
            {
                Console.WriteLine(sat.Name);
                sat.VisibleSatellites.Select(x => x.Satellite.Name).ToList().ForEach(x => Console.Write(x + " "));
                Console.WriteLine();
            }

            Console.WriteLine("Routes: ");
            foreach (var route in data.Routes)
            {
                Console.WriteLine(route);
            }
            Console.ReadLine();
        }        
    }
}
