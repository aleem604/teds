using Amazon.DynamoDBv2.DataModel;
using System;

namespace TedsProject.Models
{
    [DynamoDBTable("error_logging")]
    public class ErrorLogginModel
    {
        [DynamoDBProperty("country")]
        public string Country { get; set; }

        [DynamoDBProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [DynamoDBProperty("error_message")]
        public string Message { get; set; }

        [DynamoDBProperty("inner_exception")]
        public string InnerException { get; set; }

        [DynamoDBProperty("stack_trace")]
        public string StackTrace { get; set; }

        [DynamoDBProperty("log_date")]
        public DateTime LogDate { get; set; } = DateTime.UtcNow;

        [DynamoDBProperty("app_key")]
        public string AppKey { get; set; } 
        
        [DynamoDBProperty("ip_address")]
        public string IpAddress { get; set; }

        [DynamoDBProperty("user_id")]
        public string UserId { get; set; }
        
        [DynamoDBProperty("method_name")]
        public string MethodName { get; set; }
    }
}
