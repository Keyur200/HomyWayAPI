using HomyWayAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomyWayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {

        private readonly HomyWayContext _context;

        public ProtectedController(HomyWayContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        //[Authorize(Roles = "1")]
        public IActionResult getData()
        {
            var email = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            return Ok(new { mes = "This is protected data", user });
        }
    }
}
