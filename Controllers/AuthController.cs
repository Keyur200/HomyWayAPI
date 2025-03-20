using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomyWayAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HomyWayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HomyWayContext _context;
        private readonly IConfiguration _config;

        public AuthController(HomyWayContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (_context.Users.Any(u => u.Name == user.Name)) return BadRequest("User already exists");

            var nuser = new User
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            };

            _context.Users.Add(nuser);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");

        }

        [HttpPost("login")]
        public IActionResult Login(User users)
        {
            var Jwt = _config.GetSection("Jwt");
            var user = _context.Users.SingleOrDefault(u => u.Email == users.Email); 
            if (user == null || !BCrypt.Net.BCrypt.Verify(users.Password, user.Password)) return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(users.Email);
            return Ok(new { token, user.Name });

        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
        };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
