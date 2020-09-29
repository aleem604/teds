using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TedsProject.Interfaces;

namespace TedsProject.Controllers
{
    [Route("api/keys")]
    public class KeysController : ApiController
    {
        private readonly IKeysService _keysService;
        private readonly ILogger<KeysController> _logger;
        private readonly ILoggingService _logging;

        public KeysController(
            IKeysService keysService, 
            ILogger<KeysController> logger, 
            IHttpContextAccessor httpContext, 
            ILoggingService logging) : base(httpContext)
        {

            _keysService = keysService;
            _logger = logger;
            _logging = logging;
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKey(string id)
        {
            return Response(await _keysService.DeleteKey(id));
        }

    }
}
