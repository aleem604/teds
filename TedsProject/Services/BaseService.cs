using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TedsProject.Application;
using TedsProject.Models;

namespace TedsProject.Services
{
    public class BaseService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public BaseService(IHttpContextAccessor httpContext, IConfiguration config, IWebHostEnvironment env)
        {
            _httpContext = httpContext;
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

        public ErrorLogginModel GetErrorLogging
        {
            get
            {
                var loggingModel = new ErrorLogginModel();
                var input = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString();
                var ipAddress = input.Substring(input.LastIndexOf(':') + 1);

                loggingModel.Country = "CA";
                loggingModel.IpAddress = ipAddress;
                loggingModel.AppKey = this.GetAppKey;
                loggingModel.UserId = this.GetUserId;
                loggingModel.LogDate = DateTime.UtcNow;
                loggingModel.MethodName = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name;
                return loggingModel;
            }
        }

        public string GetUserId
        {
            get
            {
                string userid = string.Empty;

                if (_httpContext.HttpContext.Items.TryGetValue("user", out object claims))
                {
                    if (claims is AuthClaims && claims != null)
                    {
                        var authClaims = (AuthClaims)claims;
                        userid = authClaims.Id;
                    }
                }

                return userid;
            }
        }

        public string GetAppKey
        {
            get
            {
                string apiKey = string.Empty;

                if (_httpContext.HttpContext.Items.TryGetValue("user", out object claims))
                {
                    if (claims is AuthClaims && claims != null)
                    {
                        var authClaims = (AuthClaims)claims;
                        apiKey = authClaims.ApiKey;
                    }
                }

                if (string.IsNullOrEmpty(apiKey) && _httpContext.HttpContext.Request.Headers.TryGetValue("x-api-key", out StringValues key))
                {
                    apiKey = key.ToString();
                }

                return apiKey;

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
