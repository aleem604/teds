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
    [Route("api/keys")]
    public class KeysControllerController : ApiController
    {

        private readonly IKeysService _keysService;
        private readonly ILogger<KeysControllerController> _logger;

        public KeysControllerController(IKeysService keysService, ILogger<KeysControllerController> logger)
        {
            _keysService = keysService;
            _logger = logger;

        }

        [HttpGet("all-keys")]
        public async Task<IActionResult> GetAll()
        {
            return Response(await _keysService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Response(await _keysService.GetById(id));
        }
        
        [HttpGet("app-key")]
        public async Task<IActionResult> GetAppkeyByUser()
        {
            return Response(await _keysService.GetAppkeyByUser(User.Identity.Name));
        }

        [HttpPost]
        public async Task<IActionResult> SaveKey()
        {
           
            return Response(await _keysService.SaveItem(User.Identity.Name));
        }

        //[HttpPut]
        //public async Task<IActionResult> UPdateCrossing([FromBody] KeysDataModel model)
        //{
            
        //    return Response(await _keysService.SaveItem(model));
        //}
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKey(string id)
        {
            
            return Response(await _keysService.DeleteKey(id));
        }



    }
}
