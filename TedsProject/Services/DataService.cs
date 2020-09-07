using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
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
    public class DataService : IDataService
    {
        private readonly IDbService _dbService;

        public DataService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<dynamic> CreateTable()
        {
            return await Task.FromResult(true);
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

                    await this._dbService.BatchStore<CrossingsModel>(batchStore);
                }
            }
            catch (Exception ex) { 
                return Task.FromResult(false); 
            }
            return Task.FromResult(true);
        }


        public async Task<dynamic> GetAll()
        {
            return await _dbService.GetAll<CrossingsModel>();
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

    }
}
