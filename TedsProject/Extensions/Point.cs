using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Extensions
{
    public class Point
    {
        public Point(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }

        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class GeoCoordinate
    {

        public GeoCoordinate(double lat, double lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }
        public GeoCoordinate(double lat, double lng, short altitude)
        {
            this.Latitude = lat;
            this.Longitude = lng;
            this.Altitude = altitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public short Altitude { get; set; }
    }
}
