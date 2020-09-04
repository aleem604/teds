using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TedsProject.Interfaces;
using TedsProject.Models;

namespace TedsProject.Controllers
{
    [Route("api/db")]
    public class DbControllerController : ApiController
    {

        private readonly IDataService _dataService;
        private readonly ILogger<DbControllerController> _logger;

        public DbControllerController(IDataService dataService, ILogger<DbControllerController> logger)
        {
            _dataService = dataService;
            _logger = logger;

        }

        [HttpGet("create-table")]
        public IActionResult Index()
        {
            return Response(_dataService.CreateTable());
        }

        [HttpGet("all-crossings/{appKey}")]
        public async Task<IActionResult> GetAll(string appkey)
        {
            return Response(await _dataService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
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
            
            return Response(await _dataService.SaveItem(model));
        }
        
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteCrossing(string key)
        {
            
            return Response(await _dataService.DeleteCrossing(key));
        }

        [HttpGet("search/{lat}/{lng}/{key}"), AllowAnonymous]
        public async Task<IActionResult> SearchCrossing(decimal lat, decimal lng, string key)
        {
            return Response(await _dataService.SearchBYLatLang(lat, lng, key));
        }

        [HttpGet("get-gate-status/{id}")]
        public async Task<IActionResult> GetGateStatus(string id)
        {
            return Response(await _dataService.GetGateStatus(id));
        }

        [HttpPost("update-gate-status/{id}")]
        public async Task<IActionResult> UpdateGateStatus([FromBody] GateStatus isOpen, string id)
        {
            return Response(await _dataService.UpdateGateStatus(isOpen.isOpen, id));
        }
    }

    public class GateStatus
    {
        public bool isOpen { get; set; }
    }
}
