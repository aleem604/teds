using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using TedsProject.Interfaces;
using TedsProject.Models;

namespace TedsProject.Controllers
{
    [Route("api/db")]
    public class DbControllerController : ApiController
    {

        private readonly IDataService _dataService;
        private readonly IKeysService _keysService;
        private readonly ILogger<DbControllerController> _logger;

        public DbControllerController(IDataService dataService, IKeysService keysService, ILogger<DbControllerController> logger)
        {
            _dataService = dataService;
            _keysService = keysService;
            _logger = logger;

        }

        [HttpGet("create-table")]
        public IActionResult Index()
        {
            return Response(_dataService.CreateTable());
        }

        [HttpGet("all-crossings")]
        public async Task<IActionResult> GetAll()
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if(!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            return Response(await _dataService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            return Response(await _dataService.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> SaveCrossing([FromBody] CrossingsModel model)
        {
            return Response(await _dataService.SaveItem(model));
        }

        [HttpPut]
        public async Task<IActionResult> UPdateCrossing([FromBody] CrossingsModel model)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            return Response(await _dataService.SaveItem(model));
        }
        
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteCrossing(string key)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            return Response(await _dataService.DeleteCrossing(key));
        }

        [HttpGet("search/{lat}/{lng}/{key}")]
        public async Task<IActionResult> SearchCrossing(decimal lat, decimal lng, string key)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            return Response(await _dataService.SearchBYLatLang(lat, lng, key));
        }
        

        [HttpGet("get-gate-status/{id}")]
        public async Task<IActionResult> GetGateStatus(string id)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            return Response(await _dataService.GetGateStatus(id));
        }

        [HttpPost("update-gate-status/{id}")]
        public async Task<IActionResult> UpdateGateStatus([FromBody] GateStatus isOpen, string id)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");


            return Response(await _dataService.UpdateGateStatus(isOpen.isOpen, id));
        }
        
        
        
        [HttpGet("get-gate-status/{country}/{tcnumber}")]
        public async Task<IActionResult> GetGateStatusByTCNumber(string country, string tcnumber)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");

            return Response(await _dataService.GetGateStatusByTCNumber(country, tcnumber));
        }

        [HttpPost("update-gate-status/{country}/{tcnumber}")]
        public async Task<IActionResult> UpdateGateStatusByTCNumber([FromBody] GateStatus isOpen, string country, string tcnumber)
        {
            var isKeyValid = await _keysService.ValidateAppKey(GetAppKey);
            if (!isKeyValid)
                return Response(errorMessage: "Invalid Api Key");


            return Response(await _dataService.UpdateGateStatusByTCNumber(isOpen.isOpen, country, tcnumber));
        }
    }

    public class GateStatus
    {
        public bool isOpen { get; set; }
    }
}
