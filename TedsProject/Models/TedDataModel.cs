using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Models
{
    [DynamoDBTable("teds_crossings")]
    public class CrossingsModel
    {
        [DynamoDBHashKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("rank")]
        public int Rank { get; set; }

        [DynamoDBProperty("tc_number")]
        public long TCNUmber { get; set; } 
        
        [DynamoDBProperty("railway")]
        public string Railway { get; set; }

        [DynamoDBProperty("country")]
        public string Country { get; set; }

        [DynamoDBProperty("region")]
        public string Region { get; set; }

        [DynamoDBProperty("province")]
        public string Province { get; set; }

        [DynamoDBProperty("access")]
        public string Access { get; set; }

        [DynamoDBProperty("regulator")]
        public string Regulator { get; set; }

        [DynamoDBProperty("mile")]
        public decimal Mile { get; set; }
        
        [DynamoDBProperty("subdivision")]
        public string Subdivision { get; set; }

        [DynamoDBProperty("spur_mile")]
        public decimal? SpurMile { get; set; }

        [DynamoDBProperty("spur_name")]
        public string SpurName { get; set; }

        [DynamoDBProperty("location")]
        public string Location { get; set; }

        [DynamoDBProperty("latitude")]
        public decimal Latitude { get; set; }

        [DynamoDBProperty("longitude")]
        public decimal Longitude { get; set; }

        [DynamoDBProperty("road_authority")]
        public string RoadAuthority { get; set; }

        [DynamoDBProperty("prodection")]
        public string Protection { get; set; }

        [DynamoDBProperty("accident")]
        public int Accident { get; set; }

         [DynamoDBProperty("fatality")]
        public int Fatality { get; set; }

        [DynamoDBProperty("injury")]
        public int Injury { get; set; }

        [DynamoDBProperty("total_trains_daily")]
        public decimal TotalTrainsDaily { get; set; }

        [DynamoDBProperty("vehicles_daily")]
        public decimal VehicleDaily { get; set; }

        [DynamoDBProperty("trains_max")]
        public int TrainsMax { get; set; }

        [DynamoDBProperty("road_speed")]
        public int RoadSpeed { get; set; }

        [DynamoDBProperty("lanes")]
        public int Lanes { get; set; }

         [DynamoDBProperty("tracks")]
        public int Tracks { get; set; }

        [DynamoDBProperty("urban")]
        public bool Urban { get; set; }

        [DynamoDBProperty("gate_open")]
        public bool IsGateOpen { get; set; }

        public string IsGateOpenString { get {
                return this.IsGateOpen ? "Open" : "Closed";
            }
        }
    }
}
