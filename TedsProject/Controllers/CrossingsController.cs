using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
    [Route("api/db")]
    public class CrossingsController : ApiController
    {

        private readonly ICrossingsService _dataService;
        private readonly IKeysService _keysService;
        private readonly ILogger<CrossingsController> _logger;
        private readonly ILoggingService _logging;

        public CrossingsController(
            ICrossingsService dataService, 
            IKeysService keysService, ILogger<CrossingsController> logger,
             ILoggingService logging,
        IHttpContextAccessor httpContext) : base(httpContext)
        {
            _dataService = dataService;
            _keysService = keysService;
            _logger = logger;
            _logging = logging;
        }

        [HttpPost("upload-crossings")]
        public async Task<IActionResult> UploadCrossings(IFormFile file)
        {
            return Response(await _dataService.UploadCrossings(file));
        }

        [HttpPost("upload-crossings-new")]
        public async Task<IActionResult> UploadCrossingsNew(IFormFile file)
        {
            return Response(await _dataService.UploadCrossingsNew(file));
        }

        [HttpGet("all-crossings")]
        public async Task<IActionResult> GetAll(string country, string tucNumber)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if(!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"getall {country}, {tucNumber}");
            return Response(await _dataService.GetAll(country, tucNumber));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"getbyid {id}");
            return Response(await _dataService.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> SaveCrossing([FromBody] CrossingsModel model)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"SaveCrossing {model.Country} {model.TCNUmber}");
            return Response(await _dataService.SaveItem(model));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCrossing([FromBody] CrossingsModel model)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"UpdateCrossing {model.Country} {model.TCNUmber}");
            return Response(await _dataService.SaveItem(model));
        }
        
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteCrossing(string key)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"DeleteCrossing {key}");
            return Response(await _dataService.DeleteCrossing(key));
        }


        [HttpGet("delete-all")]
        public async Task<IActionResult> DeleteAllCrossing()
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"DeleteCrossing {GetAppKey}");
            return Response(await _dataService.DeleteAllCrossings());
        }


         [HttpGet("delete-new")]
        public async Task<IActionResult> DeleteNewCrossing()
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"DeleteNewCrossing {GetAppKey}");
            return Response(await _dataService.DeleteNewCrossings());
        }

        [HttpGet("search/{lat}/{lng}")]
        public async Task<IActionResult> SearchCrossing(decimal lat, decimal lng)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"SearchCrossing {lat}, {lng}");
            return Response(await _dataService.SearchBYLatLang(lat, lng));
        }

        [HttpGet("search/{lat}/{lng}/{radius}")]
        public async Task<IActionResult> SearchCrossingByRadius(double lat, double lng, short radius = 10)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"SearchCrossingByRadius {lat}, {lng}, {radius}");
            return Response(await _dataService.SearchBYRadius(lat, lng, radius));
        }
        

        [HttpGet("get-gate-status/{id}")]
        public async Task<IActionResult> GetGateStatus(string id)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"GetGateStatus {id}");
            return Response(await _dataService.GetGateStatus(id));
        }

        [HttpPost("update-gate-status/{id}")]
        public async Task<IActionResult> UpdateGateStatus([FromBody] GateStatus isOpen, string id)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"UpdateGateStatus {id}, {isOpen.isOpen}");
            return Response(await _dataService.UpdateGateStatus(isOpen.isOpen, id));
        }
        
            
        [HttpGet("get-gate-status/{country}/{tcnumber}")]
        public async Task<IActionResult> GetGateStatusByTCNumber(string country, string tcnumber)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"GetGateStatusByTCNumber {country} {tcnumber}");
            return Response(await _dataService.GetGateStatusByTCNumber(country, tcnumber));
        }

        [HttpPost("update-gate-status/{country}/{tcnumber}")]
        public async Task<IActionResult> UpdateGateStatusByTCNumber([FromBody] GateStatus isOpen, string country, string tcnumber)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            await _logging.SaveLog(GetLogging, $"UpdateGateStatusByTCNumber {isOpen.isOpen}, {country} {tcnumber}");
            return Response(await _dataService.UpdateGateStatusByTCNumber(isOpen.isOpen, country, tcnumber));
        }
    }

    public class GateStatus
    {
        public bool isOpen { get; set; }
    }
}
