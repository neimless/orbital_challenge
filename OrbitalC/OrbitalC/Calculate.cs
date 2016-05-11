using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbitalC
{
    public static class Calculate
    {
        private const double EarthRadius = 6371;

        public static double GroundDistanceBetweenPoints(double latA, double latB, double lonA, double lonB)
        {
            // using the Haversine formula
            var lat1 = latA * (Math.PI / 180);
            var lat2 = latB * (Math.PI / 180);
            var latDelta = (latB - latA) * (Math.PI / 180);
            var lonDelta = (lonB - lonA) * (Math.PI / 180);

            var a = (Math.Sin(latDelta / 2) * Math.Sin(latDelta / 2)) + (Math.Cos(lat1) * Math.Cos(lat2) * (Math.Sin(lonDelta / 2) * Math.Sin(lonDelta / 2)));
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var result = EarthRadius * c;
            return result;
        }
    }
}
