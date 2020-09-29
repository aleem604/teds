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
    public class LoggingService : ILoggingService
    {
        private readonly IDbService _dbService;
        public readonly DynamoDBContext _context;
        private readonly AmazonDynamoDBClient client;
        private const string tableName = "crossings";

        public LoggingService(IDbService dbService, IAmazonDynamoDB client)
        {
            this.client = new AmazonDynamoDBClient("AKIAR4FECMCZJLBIDAYK", "J/9gWSv4I4cg+snBLRfmVwHI8ndNx03l/WL8d4Zk", RegionEndpoint.CACentral1);
            _dbService = dbService;

        }

        public async Task<dynamic> GetAll(DateTime startDate, DateTime endTime)
        {
            return await _dbService.GetAll<LoggingModel>();
        }

        public async Task SaveLog(LoggingModel loggingModel, string methodName= "")
        {
            try
            {
                loggingModel.MethodName = methodName;
                await _dbService.Store<LoggingModel>(loggingModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }
    }
}
