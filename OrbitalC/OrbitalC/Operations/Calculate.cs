using System;

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

        public static double StraightDistanceBetweenSatellites(double latA, double latB, double lonA, double lonB, double altA, double altB)
        {
            //Distance between two satellites using ground routes
            double groundDistance = GroundDistanceBetweenPoints(latA, latB, lonA, lonB);

            // Segment angle from lines drawn to satellites from earth center
            double segmentAngle = 360 * (groundDistance / (2 * Math.PI * EarthRadius));
            segmentAngle = segmentAngle * (Math.PI / 180);

            //Direct distance of two satellites
            double satelliteDistance = Math.Sqrt(Math.Pow(EarthRadius + altA, 2) + Math.Pow(EarthRadius + altB, 2) - (2 * (EarthRadius + altA) * (EarthRadius + altB) * Math.Cos(segmentAngle)));
            return satelliteDistance;
        }

        public static double DistanceFromGroundToSatellite(double groundLat, double satelliteLat, double groundLon, double satelliteLon, double satelliteAlt)
        {
            double groundDistance = GroundDistanceBetweenPoints(groundLat, satelliteLat, groundLon, satelliteLon);

            // Segment angle from lines drawn to the two points
            double segmentAngle = 360 * (groundDistance / (2 * Math.PI * EarthRadius));
            segmentAngle = segmentAngle * (Math.PI / 180);

            //Direct distance of ground point and satellite
            double distance = Math.Sqrt(Math.Pow(EarthRadius, 2) + Math.Pow(EarthRadius + satelliteAlt, 2) - (2 * (EarthRadius) * (EarthRadius + satelliteAlt) * Math.Cos(segmentAngle)));
            return distance;
        }
    }
}
