using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ICartRepository _cartRepository;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ICartRepository cartRepository,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _cartRepository = cartRepository;
            _roleManager = roleManager;
        }

        // -------- REGISTER --------
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (await _userManager.FindByNameAsync(dto.Username) != null)
                return BadRequest("Username existe déjà");

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                Address = dto.Address,
                Phone = dto.Phone
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // 🔐 Ensure CLIENT role exists
            if (!await _roleManager.RoleExistsAsync("Client"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Client"));
            }

            // ✅ Assign CLIENT role
            await _userManager.AddToRoleAsync(user, "Client");

            // Reload user
            var createdUser = await _userManager.FindByNameAsync(user.UserName);

            // 🛒 Create cart
            await _cartRepository.CreateCartAsync(createdUser.Id);

            return Ok(new RegisterResponseDTO
            {
                UserId = createdUser.Id,
                Username = createdUser.UserName,
                Email = createdUser.Email,
                FullName = createdUser.FullName,
                Message = "User created + role assigned + cart created"
            });
        }


        // -------- LOGIN --------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials");

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
    };

            // Add each role
            foreach (var role in roles.Distinct())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                username = user.UserName,
                userId = user.Id,
                roles = roles
            });
        }
    }
}
