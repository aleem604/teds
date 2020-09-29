﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using TedsProject.Interfaces;
using TedsProject.Models;

namespace TedsProject.Controllers
{
    [Route("api/logging")]
    public class LoggingController : ApiController
    {

        private readonly ICrossingsService _dataService;
        private readonly IKeysService _keysService;
        private readonly ILogger<CrossingsController> _logger;

        public LoggingController(
            ICrossingsService dataService, 
            IKeysService keysService, 
            ILogger<CrossingsController> logger, 
            IHttpContextAccessor httpContext, 
            ILoggingService logging) : base(httpContext, logging)
        {
            _dataService = dataService;
            _keysService = keysService;
            _logger = logger;
        }

        [HttpGet("{startDate}/{endDate}")]
        public async Task<IActionResult> GetAll(DateTime startDate, DateTime endDate)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            return Response(await _loggingService.GetAll(startDate, endDate));
        }


    }

}
