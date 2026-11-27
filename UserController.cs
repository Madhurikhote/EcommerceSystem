using EcommerceWebAPI.Data;
using EcommerceWebAPI.DTOs;
using EcommerceWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ================== GET ALL USERS (Admin Only) ==================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Gender,
                    u.Role
                })
                .ToList();

            return Ok(users);
        }

        // ================== GET USER BY ID (Any logged-in user) ==================
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Gender,
                    u.Role
                })
                .FirstOrDefault();

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // ================== REGISTER USER ==================
        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.ConfirmPassword))
                return BadRequest("Password and ConfirmPassword are required");

            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Gender = dto.Gender,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully" });
        }

        // ================== LOGIN USER ==================
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            if (string.IsNullOrEmpty(dto.Password))
                return BadRequest("Password is required");

            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
                return NotFound("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            return Ok(new { token = token, message = "Login successful" });
        }

        // ================== UPDATE USER ==================
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UpdateUserDto dto)
        {
            var existingUser = _context.Users.Find(id);
            if (existingUser == null)
                return NotFound("User not found");

            // Update basic info
            existingUser.Name = dto.Name ?? existingUser.Name;
            existingUser.Email = dto.Email ?? existingUser.Email;
            existingUser.Gender = dto.Gender ?? existingUser.Gender;

            // Update password only if provided
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            // Update role only if provided and current user is admin
            if (!string.IsNullOrWhiteSpace(dto.Role))
                existingUser.Role = dto.Role;

            _context.SaveChanges();
            return Ok(new { message = "User updated successfully" });
        }

        // ================== DELETE USER (Admin Only) ==================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("User not found");

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok(new { message = "User deleted successfully" });
        }

        // ================== MAKE ADMIN (Admin Only) ==================
        [Authorize(Roles = "Admin")] // Only Admin can call
        [HttpPut("make-admin/{id}")]
        public IActionResult MakeAdmin(int id)
        {
            // 1️⃣ Find the user by Id
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // 2️⃣ Prevent admin from demoting themselves accidentally
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (currentUserId == id)
                return BadRequest(new { message = "You cannot change your own role" });

            // 3️⃣ Assign Admin role
            user.Role = "Admin";

            // 4️⃣ Save changes
            _context.SaveChanges();

            // 5️⃣ Return success
            return Ok(new { message = $"User {user.Name} promoted to Admin" });
        }

        // ================== JWT TOKEN GENERATION ==================
        private string GenerateJwtToken(User user)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}