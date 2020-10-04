using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Services
{
    public class BaseService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public BaseService(IHttpContextAccessor contextAccessor, IConfiguration config, IWebHostEnvironment env)
        {
            _contextAccessor = contextAccessor;
            _config = config;
            _env = env;
        }

        internal string AwsKey
        {
            get
            {
                return _config.GetValue<string>("AWS:AccessKey");
            }
        }
        internal string AwsSecretKey
        {
            get
            {
                return _config.GetValue<string>("AWS:SecretKey");
            }
        }

        internal string BucketName
        {
            get
            {
                return _config.GetValue<string>("AWS:BucketName");
            }
        }

        //internal string GetUrlLeftPart
        //{
        //    get
        //    {
        //        if (_env.IsDevelopment())
        //            return $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}/api/v1";
        //        else
        //            return $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}/Prod/api/v1";
        //    }
        //}
    }
}
