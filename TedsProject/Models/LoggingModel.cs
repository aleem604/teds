using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Models
{
    [DynamoDBTable("logging")]
    public class LoggingModel
    {
        [DynamoDBProperty("country")]
        public string Country { get; set; }
        [DynamoDBProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [DynamoDBProperty("app_key")]
        public string AppKey { get; set; }
        [DynamoDBProperty("log_date")]
        public DateTime LogDate { get; set; } = DateTime.UtcNow;
        [DynamoDBProperty("ip_address")]
        public string IPAddress { get; set; }
        
        [DynamoDBProperty("user_id")]
        public string UserId { get; set; }
        
        [DynamoDBProperty("method_name")]
        public string MethodName { get; set; }
    }
}
