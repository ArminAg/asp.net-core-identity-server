using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace asp.net_core_identity_server_with_identity.Controllers.Api
{
    [Route("api/UserManagement")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserManagementController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}