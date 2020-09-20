using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TedsProject.Extensions;
using TedsProject.Interfaces;
using TedsProject.Models;
using TedsProject.Models.Parsers;

namespace TedsProject.Services
{
    public class CrossingsService : ICrossingsService
    {
        private readonly IDbService _dbService;
        public readonly DynamoDBContext _context;
        private readonly AmazonDynamoDBClient client;
        private const string tableName = "crossings";

        public CrossingsService(IDbService dbService, IAmazonDynamoDB client)
        {
            this.client = new AmazonDynamoDBClient("AKIAR4FECMCZJLBIDAYK", "J/9gWSv4I4cg+snBLRfmVwHI8ndNx03l/WL8d4Zk", RegionEndpoint.CACentral1);
            _dbService = dbService;

        }

        public async Task<dynamic> UploadCrossings(IFormFile file)
        {
            try
            {
                var crossings = new List<CrossingsCsv>();

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var record = await reader.ReadToEndAsync();


                    TextReader sr = new StringReader(record);

                    using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
                    {
                        csv.Configuration.RegisterClassMap<CrosingsMap>();
                        var records = csv.GetRecords<CrossingsCsv>();
                        crossings = records.ToList();
                    }
                }

                if (crossings != null && crossings.Count() > 0)
                {
                    var batchStore = new List<CrossingsModel>();
                    crossings.ForEach(c =>
                    {
                        var model = (CrossingsModel)c;
                        model.Id = Guid.NewGuid().ToString();
                        model.Urban = c.UrbanString == "Y" || c.UrbanString == "y";
                        batchStore.Add(model);

                    });

                    await this._dbService.BatchStore<CrossingsModel>(batchStore.Take(500));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public async Task<dynamic> UploadCrossingsNew(IFormFile file)
        {
            try
            {
                var crossings = new List<CrossingsCsvNew>();

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var record = await reader.ReadToEndAsync();


                    TextReader sr = new StringReader(record);

                    using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
                    {
                        csv.Configuration.RegisterClassMap<CrosingsMapNew>();
                        var records = csv.GetRecords<CrossingsCsvNew>();
                        crossings = records.ToList();
                    }
                }

                if (crossings != null && crossings.Count() > 0)
                {
                    var batchStore = new List<CrossingsModelNew>();
                    crossings.ForEach(c =>
                    {
                        var model = (CrossingsModelNew)c;
                        model.Urban = c.UrbanString == "Y" || c.UrbanString == "y";
                        model.CreateDate = DateTime.UtcNow;

                        if (batchStore.Where(w => w.TCNUmber == model.TCNUmber).Count() == 0)
                            batchStore.Add(model);

                    });

                    await this._dbService.BatchStore<CrossingsModelNew>(batchStore);
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
            }
            return Task.FromResult(true);
        }


        public async Task<dynamic> GetAll(string country, string tucNumber)
        {
            try
            {

                var result = new List<dynamic>();
                var request = new QueryRequest
                {
                    TableName = tableName,
                    // KeyConditionExpression = "country = :v_Country and tc_number = :v_tucNumber",
                    KeyConditionExpression = "country = :v_Country",

                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_Country", new AttributeValue { S =  country }},
                       // {":v_tucNumber", new AttributeValue { N =  tucNumber }}
                },
                    ConsistentRead = true
                };

                var response = await this.client.QueryAsync(request);
                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    result.Add(GetItem(item));
                }
                return result;
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
            }
        }


        public async Task<dynamic> GetById(string key)
        {
            return await _dbService.GetItemsById<CrossingsModel>(key);
        }

        public async Task<dynamic> SaveItem(CrossingsModel item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
                await _dbService.Store<CrossingsModel>(item);
            }
            else
            {
                await _dbService.UpdateItem<CrossingsModel>(item);
            }
            return await Task.FromResult(true);
        }

        public async Task<dynamic> DeleteCrossing(string key)
        {
            var model = await _dbService.GetItem<CrossingsModel>(key);
            await _dbService.DeleteItem<CrossingsModel>(model);

            return await Task.FromResult(true);
        }

        public async Task<dynamic> DeleteAllCrossings()
        {
            var models = await _dbService.GetAll<CrossingsModel>();
            models.ToList().ForEach(async f =>
            {
                await _dbService.DeleteItem<CrossingsModel>(f);
            });

            return await Task.FromResult(true);
        }
         public async Task<dynamic> DeleteNewCrossings()
        {
            var models = await _dbService.GetAll<CrossingsModelNew>();
            models.ToList().ForEach(async f =>
            {
                await _dbService.DeleteItem<CrossingsModelNew>(f);
            });

            return await Task.FromResult(true);
        }

        public async Task<dynamic> SearchBYLatLang(decimal lat, decimal lng)
        {
            var scanCondition = new List<ScanCondition> {
                new ScanCondition("Latitude", ScanOperator.Equal, lat),
                new ScanCondition("Longitude", ScanOperator.LessThanOrEqual, lng)
            };

            return await _dbService.GetAll<CrossingsModel>(scanCondition);
        }

        public async Task<dynamic> SearchBYRadius(double lat, double lng, short radius)
        {
            var crossings = await _dbService.GetAll<CrossingsModel>();
            var center = new GeoCoordinate(lat, lng);
            var nradius = 111320;

            var southBound = center.CalculateDerivedPosition(nradius, -180);
            var westBound = center.CalculateDerivedPosition(nradius, -90);
            var eastBound = center.CalculateDerivedPosition(nradius, 90);
            var northBound = center.CalculateDerivedPosition(nradius * radius, 0);

            //return new {southBound, westBound, eastBound, northBound };       

            return new { crossings = crossings.Where(x => x.Latitude <= Convert.ToDecimal(northBound.Latitude)).ToList(), northBound };
        }


        public async Task<dynamic> GetGateStatus(string id)
        {
            var result = await _dbService.GetItemsById<CrossingsModel>(id);
            return result.Select(s => new { id = s.Id, GateStatus = s.IsGateOpenString }).FirstOrDefault();
        }
        public async Task<dynamic> UpdateGateStatus(bool status, string id)
        {
            var items = await _dbService.GetItemsById<CrossingsModel>(id);
            if (items != null && items.Count() > 0)
            {
                var item = items.FirstOrDefault();
                item.IsGateOpen = status;
                await _dbService.UpdateItem<CrossingsModel>(item);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }


        public async Task<dynamic> GetGateStatusByTCNumber(string country, string tcnumber)
        {

            var scanCondition = new List<ScanCondition> {
                new ScanCondition("Country", ScanOperator.Equal, country?.ToUpper()),
                new ScanCondition("TCNUmber", ScanOperator.Equal, Int64.Parse(tcnumber))
            };

            var result = await _dbService.GetAll<CrossingsModel>(scanCondition);

            return result.Select(s => new { id = s.Id, GateStatus = s.IsGateOpenString }).FirstOrDefault();
        }
        public async Task<dynamic> UpdateGateStatusByTCNumber(bool status, string country, string tcnumber)
        {
            var scanCondition = new List<ScanCondition> {
                new ScanCondition("Country", ScanOperator.Equal, country?.ToUpper()),
                new ScanCondition("TCNUmber", ScanOperator.Equal, Int64.Parse(tcnumber))
            };

            var items = await _dbService.GetAll<CrossingsModel>(scanCondition);

            if (items != null && items.Count() > 0)
            {
                var item = items.FirstOrDefault();
                item.IsGateOpen = status;
                await _dbService.UpdateItem<CrossingsModel>(item);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        private async Task<bool> isValidKey(string key)
        {
            var scanCondition = new List<ScanCondition> {
                new ScanCondition("AppKey", ScanOperator.Equal, key)
            };

            var result = await _dbService.GetAll<KeysDataModel>(scanCondition) ?? new List<KeysDataModel>();
            var item = result.Where(w => w.ExpiryDate > DateTime.UtcNow).OrderByDescending(o => o.ExpiryDate).FirstOrDefault();

            return item != null;
        }

        private static CrossingsModelNew GetItem(Dictionary<string, AttributeValue> item)
        {
            var model = new CrossingsModelNew();
            model.Country = item.ContainsKey("country") ? item["country"].S : default;
            model.TCNUmber = item.ContainsKey("tc_number") ? Convert.ToInt64(item["tc_number"].N) : default;
            model.Rank = item.ContainsKey("rank") ? Convert.ToInt32(item["rank"].N) : default;
            model.Railway = item.ContainsKey("railway") ? item["railway"].S : default;
            model.Region = item.ContainsKey("region") ? item["region"].S : default;
            model.Province = item.ContainsKey("province") ? item["province"].S : default;
            model.Access = item.ContainsKey("access") ? item["access"].S : default;
            model.Regulator = item.ContainsKey("regulator") ? item["regulator"].S : default;
            model.Mile = item.ContainsKey("mile") ? Convert.ToDecimal(item["mile"].N) : default;
            model.Subdivision = item.ContainsKey("subdivision") ? item["subdivision"].S : default;
            model.SpurMile = item.ContainsKey("spur_mile") ? Convert.ToDecimal(item["spur_mile"].N) : default;
            model.SpurName = item.ContainsKey("spur_name") ? item["spur_name"].S : default;
            model.Location = item.ContainsKey("location") ? item["location"].S : default;
            model.Latitude = item.ContainsKey("latitue") ? Convert.ToDecimal(item["latitue"].N) : default;
            model.Longitude = item.ContainsKey("longitude") ? Convert.ToDecimal(item["longitude"].N) : default;
            model.RoadAuthority = item.ContainsKey("road_authority") ? item["road_authority"].S : default;
            model.Protection = item.ContainsKey("prodection") ? item["prodection"].S : default;
            model.Accident = item.ContainsKey("accident") ? Convert.ToInt32(item["accident"].N) : default;
            model.Fatality = item.ContainsKey("fatality") ? Convert.ToInt32(item["fatality"].N) : default;
            model.Injury = item.ContainsKey("injury") ? Convert.ToInt32(item["injury"].N) : default;
            model.TotalTrainsDaily = item.ContainsKey("total_trains_daily") ? Convert.ToDecimal(item["total_trains_daily"].N) : default;
            model.VehicleDaily = item.ContainsKey("vehicles_daily") ? Convert.ToDecimal(item["vehicles_daily"].N) : default;
            model.TrainsMax = item.ContainsKey("trains_max") ? Convert.ToInt32(item["trains_max"].N) : default;
            model.RoadSpeed = item.ContainsKey("road_speed") ? Convert.ToInt32(item["road_speed"].N) : default;
            model.Lanes = item.ContainsKey("lanes") ? Convert.ToInt32(item["lanes"].N) : default;
            model.Tracks = item.ContainsKey("tracks") ? Convert.ToInt32(item["tracks"].N) : default;
            model.Urban = item.ContainsKey("urban") ? item["urban"].N == "0" ? false : true : default;
            model.IsGateOpen = item.ContainsKey("gate_open") ? Convert.ToBoolean(item["gate_open"].N) : default;
            return model;
        }


    }
}
