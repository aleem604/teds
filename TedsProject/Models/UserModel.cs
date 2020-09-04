using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Models
{
    [DynamoDBTable("admin_table")]
    public class UserModel
    {
        [DynamoDBHashKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("first_name")]
        public string FirstName { get; set; }
        [DynamoDBProperty("last_name")]
        public string LastName { get; set; }
        [DynamoDBProperty("email_address")]
        public string Email { get; set; }
        [DynamoDBProperty("password")]
        public string Password { get; set; }

        [DynamoDBProperty("update_date")]
        public DateTime UpdateDate { get; set; } 
        [DynamoDBProperty("createa_at")]
        public DateTime CreateAt { get; set; } 

     
    }
}
