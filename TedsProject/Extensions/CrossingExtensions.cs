using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Extensions
{
    public static class CrossingExtensions
    {
        public static Point ExtendPoint(Point _pt, int _distance, int _bearing)
        {
            double lat = 0.0d;
            double lng = 0.0d;

            lat = Math.Asin(Math.Sin(_pt.Lat) * Math.Cos(_distance) + Math.Cos(_pt.Lat) *
                Math.Sin(_distance) * Math.Cos(_bearing));

            if (Math.Cos(lat) == 0)
            {
                lng = _pt.Lng;      // endpoint a pole
            }
            else
            {
                lng = ((_pt.Lng - Math.Asin(Math.Sin(_bearing) * Math.Sin(_distance) / Math.Cos(lat)) + Math.PI) % (2 * Math.PI)) - Math.PI;
            }

           var ret = new Point(lat, lng);
            return ret;
        }

        public static GeoCoordinate CalculateDerivedPosition(this GeoCoordinate source, double range, double bearing)
        {
            var latA = source.Latitude * DegreesToRadians;
            var lonA = source.Longitude * DegreesToRadians;
            var angularDistance = range / EarthRadius;
            var trueCourse = bearing * DegreesToRadians;

            var lat = Math.Asin(
                Math.Sin(latA) * Math.Cos(angularDistance) +
                Math.Cos(latA) * Math.Sin(angularDistance) * Math.Cos(trueCourse));

            var dlon = Math.Atan2(
                Math.Sin(trueCourse) * Math.Sin(angularDistance) * Math.Cos(latA),
                Math.Cos(angularDistance) - Math.Sin(latA) * Math.Sin(lat));

            var lon = ((lonA + dlon + Math.PI) % (Math.PI * 2)) - Math.PI;

            return new GeoCoordinate(
                lat * RadiansToDegrees,
                lon * RadiansToDegrees,
                source.Altitude);
        }

        private const double DegreesToRadians = Math.PI / 180.0;
        private const double RadiansToDegrees = 180.0 / Math.PI;
        private const double EarthRadius = 6378137.0;
    }
}
