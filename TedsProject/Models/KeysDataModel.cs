using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Models
{
    [DynamoDBTable("keys_table")]
    public class KeysDataModel
    {
        [DynamoDBHashKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("user_id")]
        public string UserId { get; set; } 

        [DynamoDBProperty("app_key")]
        public string AppKey { get; set; }

        [DynamoDBProperty("expiry_date")]
        public DateTime ExpiryDate { get; set; } 
        [DynamoDBProperty("create_date")]
        public DateTime CreateDate { get; set; } 

     
    }
}
