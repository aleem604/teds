using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Models.Parsers
{
    public class CrossingsCsv : CrossingsModel
    {
        public string UrbanString { get; set; }

    }

    public sealed class CrosingsMap : ClassMap<CrossingsCsv>
    {
        public CrosingsMap()
        {
            Map(x => x.Rank).Name("Rank");
            Map(x => x.TCNUmber).Name("TC Number");
            Map(x => x.Railway).Name("Railway");
            Map(x => x.Region).Name("Region");
            Map(x => x.Country).Name("Country");
            Map(x => x.Province).Name("Province");
            Map(x => x.Access).Name("Access");
            Map(x => x.Regulator).Name("Regulator");
            Map(x => x.Mile).Name("Mile");
            Map(x => x.Subdivision).Name("Subdivision");
            Map(x => x.SpurMile).Name("Spur Mile");
            Map(x => x.SpurName).Name("Spur Name");
            Map(x => x.Location).Name("Location");
            Map(x => x.Latitude).Name("Latitude");
            Map(x => x.Longitude).Name("Longitude");
            Map(x => x.RoadAuthority).Name("Road Authority");
            Map(x => x.Protection).Name("Protection");
            Map(x => x.Accident).Name("Accident");
            Map(x => x.Fatality).Name("Fatality");
            Map(x => x.Injury).Name("Injury");
            Map(x => x.Injury).Name("Injury");
            Map(x => x.TotalTrainsDaily).Name("Total Trains Daily");
            Map(x => x.VehicleDaily).Name("Vehicles Daily");
            Map(x => x.TrainsMax).Name("Train Max Speed (mph)");
            Map(x => x.RoadSpeed).Name("Road Speed (km/h)");
            Map(x => x.Lanes).Name("Lanes");
            Map(x => x.Tracks).Name("Tracks");
            Map(x => x.UrbanString).Name("Urban Y/N");
        }
    }
}
