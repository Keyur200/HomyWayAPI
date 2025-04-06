using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomyWayAPI.DTO;
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
        public async Task<IActionResult> Register(UserDTO userdto)
        {
            if (_context.Users.Any(u => u.Name == userdto.Name)) return BadRequest("User already exists");
            var groupExists =await _context.Groups.AnyAsync(g=>g.Id == userdto.Gid);
            if (!groupExists)
            {
                return BadRequest("Invalid Group ID");
            }

            var nuser = new User
            {
                Id = 0, 
                Name = userdto.Name,
                Email = userdto.Email,
                Phone = userdto.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(userdto.Password),
                Gid = userdto.Gid,
            };

            _context.Users.Add(nuser);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");

        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO users)
        {
            var Jwt = _config.GetSection("Jwt");
            var user = _context.Users.SingleOrDefault(u => u.Email == users.Email); 
            if (user == null || !BCrypt.Net.BCrypt.Verify(users.Password, user.Password)) return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(users.Email);
            return Ok(new { token, user = new { user.Name, user.Email, user.Phone, user.Gid } });

        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var user = _context.Users.FirstOrDefault(u => u.Email == username);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, user.Gid.ToString()),
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
