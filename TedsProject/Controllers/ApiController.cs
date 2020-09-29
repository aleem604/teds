using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TedsProject.Application;
using TedsProject.Interfaces;
using TedsProject.Models;

namespace TedsProject.Controllers
{

    public abstract class ApiController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContext;
        protected ApiController(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        protected new IActionResult Response(object result = null, string errorMessage = null)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    data = errorMessage
                });

            }
        }

        protected void NotifyModelStateErrors()
        {
            var erros = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var erro in erros)
            {
                var erroMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                //  NotifyError(string.Empty, erroMsg);
            }
        }

        protected IEnumerable<string> GetModelStateErrors()
        {
            var errors = ModelState.Select(x => x.Value.Errors)
                                 .Where(y => y.Count > 0).SelectMany(x => x.Select(y => y.ErrorMessage))
                                 .ToList();
            return errors.ToList();
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

                if (string.IsNullOrEmpty(apiKey) && Request.Headers.TryGetValue("x-api-key", out StringValues key))
                {
                    apiKey = key.ToString();
                }
               
                return apiKey;

            }
        }

        public LoggingModel GetLogging
        {
            get
            {
                var loggingModel = new LoggingModel();
                var input = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString();
                var ipAddress = input.Substring(input.LastIndexOf(':') + 1);

                loggingModel.Country = "CA";
                loggingModel.IPAddress = ipAddress;
                loggingModel.AppKey = this.GetAppKey;
                loggingModel.UserId = this.GetUserId;
                loggingModel.LogDate = DateTime.UtcNow;
                loggingModel.MethodName = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name;
                return loggingModel;
            }
        }

    }
}
