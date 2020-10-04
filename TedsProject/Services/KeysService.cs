using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TedsProject.Interfaces;
using TedsProject.Models;

namespace TedsProject.Services
{
    public class KeysService : BaseService, IKeysService
    {
        private readonly IDbService _dbService;
        public readonly DynamoDBContext _context;
        private readonly AmazonDynamoDBClient client;
        private const string tableName = "keys_table";

        public KeysService(IDbService dbService, IConfiguration config, IHttpContextAccessor httpContext, IWebHostEnvironment env) : base(httpContext, config, env)
        {
            this.client = new AmazonDynamoDBClient(AwsKey, AwsSecretKey, RegionEndpoint.CACentral1);
            _dbService = dbService;
        }


        public async Task<dynamic> GetAll()
        {
            return await _dbService.GetAll<KeysDataModel>();
        }

        public async Task<dynamic> GetById(string key)
        {
            return await _dbService.GetItemsById<KeysDataModel>(key);
        }

        public async Task<dynamic> GetAppkeyByUser(string userId)
        {
            var scanCodition = new List<ScanCondition> {
                     new ScanCondition("UserId", ScanOperator.Equal, userId),
                     new ScanCondition("ExpiryDate", ScanOperator.GreaterThanOrEqual, DateTime.UtcNow),
                };

            var result = await _dbService.GetAll<KeysDataModel>(scanCodition) ?? new List<KeysDataModel>();
            return result.Select(s => new { s.Id, s.AppKey }).FirstOrDefault();
        }
        
        
        public async Task<dynamic> SaveItem(string userId)
        {
            var item = new KeysDataModel();
            item.UserId = userId;
            item.Id = Guid.NewGuid().ToString();
            item.AppKey = Guid.NewGuid().ToString();
            item.ExpiryDate = DateTime.UtcNow.AddDays(21);
            item.CreateDate = DateTime.UtcNow;
            await _dbService.Store<KeysDataModel>(item);

            return await Task.FromResult(true);
        }

        public async Task<dynamic> DeleteKey(string key)
        {
            var model = await _dbService.GetItem<KeysDataModel>(key);
            await _dbService.DeleteItem<KeysDataModel>(model);

            return await Task.FromResult(true);
        }

        public async Task<bool> ValidateAppKey(string key)
        {
            if(string.IsNullOrEmpty(key))
                return await Task.FromResult(false);

            var scanCodition = new List<ScanCondition> {
                     new ScanCondition("AppKey", ScanOperator.Equal, key)
                };

            var result = await _dbService.GetAll<KeysDataModel>(scanCodition);

            if (result?.Count() >0 && result.FirstOrDefault()?.ExpiryDate > DateTime.UtcNow)
                return await Task.FromResult(true);
            else
                return await Task.FromResult(false);
        }


    }
}
