using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomyWayAPI.DTO;
using HomyWayAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userdto)
        {
            if (_context.Users.Any(u => u.Name == userdto.Name)) return BadRequest("User already exists");
            var groupExists =await _context.Groups.AnyAsync(g=>g.Id == userdto.Gid);

            //Check if group exists
            if (!groupExists)
            {
                return BadRequest("Invalid Group ID");
            }

            //Created object of User model 
            var nuser = new User
            {
                Id = 0, 
                Name = userdto.Name,
                Email = userdto.Email,
                Phone = userdto.Phone,
                Status = userdto.Status,
                Password = BCrypt.Net.BCrypt.HashPassword(userdto.Password),
                Gid = userdto.Gid,
            };

            //Save into Database 
            _context.Users.Add(nuser);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");

        }

        //Login user code 

        [HttpPost("login")]
        public IActionResult Login(LoginDTO users)
        {
            var Jwt = _config.GetSection("Jwt");
            var user = _context.Users.SingleOrDefault(u => u.Email == users.Email);
            
            //check user is ExistsExpression or not 
            if (user == null || !BCrypt.Net.BCrypt.Verify(users.Password, user.Password)) return Unauthorized("Invalid credentials");

            //check status of user 
            if (user.Status == "pending") return Unauthorized("Please wait for approval");
            if (user.Status == "block") return Unauthorized("Your account has been block");

            //valid user 
            var token = GenerateJwtToken(users.Email);
            return Ok(new { token, user = new { user.Name, user.Email, user.Phone, user.Gid } });

        }

        //Update status code
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromQuery] string status)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found");
            user.Status = status;
            await _context.SaveChangesAsync();
            return Ok("User status updated successfully");
        }

        //generate jwt token code
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
