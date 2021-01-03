using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TedsProject.Extensions;
using TedsProject.Interfaces;
using TedsProject.Models;

namespace TedsProject.Services
{
    public class ErrorLogService : BaseService, IErrorLogService
    {
        private readonly IDbService _dbService;
        public readonly DynamoDBContext _context;
        private readonly AmazonDynamoDBClient client;
        private const string tableName = "error_logging";

        public ErrorLogService(IDbService dbService, IConfiguration config, IHttpContextAccessor httpContext, IWebHostEnvironment env) : base(httpContext, config, env)
        {
            client = new AmazonDynamoDBClient(AwsKey, AwsSecretKey, RegionEndpoint.CACentral1);
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

        public async Task SaveExceptionLog(Exception ex, string methodName = "")
        {
            try
            {
                var model = new ErrorLogginModel();
                model.UserId = GetUserId;
                model.AppKey = GetAppKey;
                model.MethodName = methodName;
                model.Message = ex.Message;
                model.InnerException = ex.FullMessage();
                model.StackTrace = ex.StackTrace;

                await _dbService.Store<ErrorLogginModel>(model);
            }
            catch (Exception iex)
            {
                Console.WriteLine(iex);
            }
        }

       
        private static ErrorLogginModel GetItem(Dictionary<string, AttributeValue> item)
        {
            var model = new ErrorLogginModel
            {
                Country = item.ContainsKey("country") ? item["country"].S : default,
                Id = item.ContainsKey("id") ? item["id"].S : default,
                LogDate = item.ContainsKey("log_date") ? Convert.ToDateTime(item["log_date"].S) : default,
                AppKey = item.ContainsKey("app_key") ? item["app_key"].S : default,
                IpAddress = item.ContainsKey("ip_address") ? item["ip_address"].S : default,
                UserId = item.ContainsKey("user_id") ? item["user_id"].S : default,
                MethodName = item.ContainsKey("method_name") ? item["method_name"].S : default,
                Message = item.ContainsKey("error_message") ? item["error_message"].S : default,
                StackTrace = item.ContainsKey("stack_trace") ? item["stack_trace"].S : default,
                InnerException = item.ContainsKey("inner_exception") ? item["inner_exception"].S : default
            };
            return model;
        }
    }
}
