using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomyWayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {

        [HttpGet]
        //[Authorize]
        [Authorize(Roles = "1")]
        public IActionResult getData()
        {
            var name = User.Identity?.Name;
            return Ok(new { mes = "This is protected data", name });
        }
    }
}
