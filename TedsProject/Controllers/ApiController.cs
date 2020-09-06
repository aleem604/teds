using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace TedsProject.Controllers
{

    public abstract class ApiController : ControllerBase
    {
        protected ApiController()
        {

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

        public string GetAppKey
        {
            get
            {

                if (Request.Headers.TryGetValue("x-api-key", out StringValues key))
                {
                    return key.ToString();
                }
                return string.Empty;

            }
        }
    }
}
