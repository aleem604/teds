using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
    public class CrossingsService : BaseService, ICrossingsService
    {
        private readonly IDbService _dbService;
        private readonly IErrorLogService _errorService;
        public readonly DynamoDBContext _context;
        private readonly AmazonDynamoDBClient client;
        private const string tableName = "crossings";

        public CrossingsService(IDbService dbService, IErrorLogService errorService, IConfiguration config, IHttpContextAccessor httpContext, IWebHostEnvironment env) : base(httpContext, config, env)
        {
            this.client = new AmazonDynamoDBClient(AwsKey, AwsSecretKey, RegionEndpoint.CACentral1);
            _dbService = dbService;
            _errorService = errorService;
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
                await _errorService.SaveExceptionLog(ex, "crossings uploadcrossings");
                return Task.FromResult(false);
            }
            return await Task.FromResult(true);
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
                await _errorService.SaveExceptionLog(ex, "crossings uploadcrossingsnew");
                return Task.FromResult(ex.Message);
            }
            return await Task.FromResult(true);
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
                await _errorService.SaveExceptionLog(ex, "crossings getall");
                return await Task.FromResult(ex.Message);
            }
        }


        public async Task<dynamic> GetById(string key)
        {
            try
            {
                return await _dbService.GetItemsById<CrossingsModel>(key);
            }
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"crossings getbyid {key}");
                return await Task.FromResult(ex.Message);
            }
        }

        public async Task<dynamic> SaveItem(CrossingsModel item)
        {
            try
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
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings SaveItem {item.Country} {item.TCNUmber}");
                return await Task.FromResult(ex.Message);
            }
        }

        public async Task<dynamic> DeleteCrossing(string key)
        {
            try
            {
                var model = await _dbService.GetItem<CrossingsModel>(key);
                await _dbService.DeleteItem<CrossingsModel>(model);

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings DeleteCrossing {key}");
                return await Task.FromResult(ex.Message);
            }
        }

        public async Task<dynamic> DeleteAllCrossings()
        {
            try
            {
                var models = await _dbService.GetAll<CrossingsModel>();
                models.ToList().ForEach(async f =>
                {
                    await _dbService.DeleteItem<CrossingsModel>(f);
                });

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings DeleteAllCrossings");
                return await Task.FromResult(ex.Message);
            }
        }
        public async Task<dynamic> DeleteNewCrossings()
        {
            try
            {
                var models = await _dbService.GetAll<CrossingsModelNew>();
                models.ToList().ForEach(async f =>
                {
                    await _dbService.DeleteItem<CrossingsModelNew>(f);
                });

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings DeleteCrossing");
                return await Task.FromResult(ex.Message);
            }
        }

        public async Task<dynamic> SearchBYLatLang(decimal lat, decimal lng)
        {
            try
            {
                var scanCondition = new List<ScanCondition> {
                new ScanCondition("Latitude", ScanOperator.Equal, lat),
                new ScanCondition("Longitude", ScanOperator.LessThanOrEqual, lng)
            };

                return await _dbService.GetAll<CrossingsModel>(scanCondition);
            }
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings SearchBYLatLang");
                return await Task.FromResult(ex.Message);
            }
        }

        //public async Task<dynamic> SearchBYRadius(double lat, double lng, int radius)
        //{
        //    try
        //    {
        //        var crossings = await _dbService.GetAll<CrossingsModel>();
        //        var center = new GeoCoordinate(lat, lng);
        //        var nradius = 111320;

        //        var southBound = center.CalculateDerivedPosition(nradius, -180);
        //        var westBound = center.CalculateDerivedPosition(nradius, -90);
        //        var eastBound = center.CalculateDerivedPosition(nradius, 90);
        //        var northBound = center.CalculateDerivedPosition(nradius * radius, 0);

        //        return new { crossings = crossings.Where(x => x.Latitude <= Convert.ToDecimal(northBound.Latitude)).ToList(), northBound };
        //    }
        //    catch (Exception ex)
        //    {
        //        await _errorService.SaveExceptionLog(ex, $"Crossings SearchBYRadius {lat} {lng} {radius}");
        //        return await Task.FromResult(ex.Message);
        //    }
        //}


        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="radius">KM</param>
        /// verified by https://www.geodatasource.com/distance-calculator
        /// <returns></returns>
        public async Task<dynamic> SearchBYRadius(double lat, double lng, int radius)
        {
            try
            {
                var crossings = await _dbService.GetAll<CrossingsModel>();
                var records = new List<CrossingsModel>();
                foreach (var crossing in crossings)
                {
                    var distance = new Coordinates(lat, lng).DistanceTo(new Coordinates(Convert.ToDouble(crossing.Latitude), Convert.ToDouble(crossing.Longitude)),UnitOfLength.Kilometers);

                    if (distance <= radius)
                        records.Add(crossing);
                }

                return new { crossings = records };
            }
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings SearchBYRadius {lat} {lng} {radius}");
                return await Task.FromResult(ex.Message);
            }
        }



        public async Task<dynamic> GetGateStatus(string id)
        {
            try
            {
                var result = await _dbService.GetItemsById<CrossingsModel>(id);
                return result.Select(s => new { id = s.Id, GateStatus = s.IsGateOpenString }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings GetGateStatus {id}");
                return await Task.FromResult(ex.Message);
            }
        }
        public async Task<dynamic> UpdateGateStatus(bool status, string id)
        {
            try
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
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings UpdateGateStatus {status} {id}");
                return await Task.FromResult(ex.Message);
            }
        }


        public async Task<dynamic> GetGateStatusByTCNumber(string country, string tcnumber)
        {
            try
            {
                var scanCondition = new List<ScanCondition> {
                new ScanCondition("Country", ScanOperator.Equal, country?.ToUpper()),
                new ScanCondition("TCNUmber", ScanOperator.Equal, Int64.Parse(tcnumber))
            };

                var result = await _dbService.GetAll<CrossingsModel>(scanCondition);

                return result.Select(s => new { id = s.Id, GateStatus = s.IsGateOpenString }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings GetGateStatusByTCNumber {country} {tcnumber}");
                return await Task.FromResult(ex.Message);
            }
        }
        public async Task<dynamic> UpdateGateStatusByTCNumber(bool status, string country, string tcnumber)
        {
            try
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
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings UpdateGateStatusByTCNumber {status} {country} {tcnumber}");
                return await Task.FromResult(ex.Message);
            }
        }

        private async Task<bool> isValidKey(string key)
        {
            try
            {
                var scanCondition = new List<ScanCondition> {
                new ScanCondition("AppKey", ScanOperator.Equal, key)
            };

                var result = await _dbService.GetAll<KeysDataModel>(scanCondition) ?? new List<KeysDataModel>();
                var item = result.Where(w => w.ExpiryDate > DateTime.UtcNow).OrderByDescending(o => o.ExpiryDate).FirstOrDefault();

                return item != null;
            }
            catch (Exception ex)
            {
                await _errorService.SaveExceptionLog(ex, $"Crossings DeleteCrossing {key}");
                return await Task.FromResult(false);
            }
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

        public double gpsCordDistance(double pointLat, double pointLon, double locLat, double locLon)
        {
            double R = 6371; // Earth Radius kilometers
            double dLat = degreesToRadians(pointLat - locLat);
            double dLon = degreesToRadians(pointLon - locLon);
            double lat1 = degreesToRadians(locLat);
            double lat2 = degreesToRadians(pointLat);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;

            return d;
        }

        public double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        private double degreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

    }
}
