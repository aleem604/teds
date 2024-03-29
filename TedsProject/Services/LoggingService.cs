﻿using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Util;
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
    public class LoggingService :BaseService, ILoggingService
    {
        private readonly IDbService _dbService;
        public readonly DynamoDBContext _context;
        private readonly AmazonDynamoDBClient client;
        private const string tableName = "logging";

        public LoggingService(IDbService dbService, IConfiguration config, IHttpContextAccessor httpContext, IWebHostEnvironment env) : base(httpContext, config, env)
        {
            this.client = new AmazonDynamoDBClient(AwsKey, AwsSecretKey, RegionEndpoint.CACentral1);
            _dbService = dbService;
        }

        public async Task<dynamic> GetAll(DateTime startDate, DateTime endTime)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
            dateFormatter.TimeZone = TimeZoneInfo.Utc;
            try
            {

                var result = new List<dynamic>();
                var request = new QueryRequest
                {
                    TableName = tableName,
                    // KeyConditionExpression = "country = :v_Country and tc_number = :v_tucNumber",
                    KeyConditionExpression = "country = :v_Country",
                    FilterExpression = "log_date >= :startDate and log_date <= :endDate",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_Country", new AttributeValue { S =  "CA" }},
                        {":startDate", new AttributeValue { S =  dateFormatter.Format(startDate) }},
                        {":endDate", new AttributeValue { S =  dateFormatter.Format(endTime) }}
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

        public async Task SaveLog(LoggingModel loggingModel, string methodName = "")
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

        private static LoggingModel GetItem(Dictionary<string, AttributeValue> item)
        {
            var model = new LoggingModel();
            model.Country = item.ContainsKey("country") ? item["country"].S : default;
            model.LogDate = item.ContainsKey("log_date") ? Convert.ToDateTime(item["log_date"].S) : default;
            model.AppKey = item.ContainsKey("app_key") ? item["app_key"].S : default;
            model.UserId = item.ContainsKey("user_id") ? item["user_id"].S : default;
            model.IPAddress = item.ContainsKey("ip_address") ? item["ip_address"].S : default;
            model.MethodName = item.ContainsKey("method_name") ? item["method_name"].S : default;
            return model;
        }
    }
}
