using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TedsProject.Interfaces;
using TedsProject.Models;

namespace TedsProject.Controllers
{
    [Route("api/user"), Authorize]
    public class AdminController : ApiController
    {

        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAdminService adminService, 
            ILogger<AdminController> logger, 
            IHttpContextAccessor httpContext) : base(httpContext)
        {
            _adminService = adminService;
            _logger = logger;

        }

        // GET: api/User/register
        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = GetModelStateErrors();
                return Response("", string.Join("<br/>", errors));
            }
            var response = await _adminService.Register(model);
            return Response(response.Item1, response.Item2);
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = GetModelStateErrors();
                return Response("", string.Join("<br/>", errors));
            }
            var response = await _adminService.Login(model);
            
            return Response(response.Item1, response.Item2);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordViewModel model)
        {
            var response = await _adminService.ChangePassword(model, User.Identity.Name);
            return Response(response.Item1, response.Item2);
        }





    }
}
