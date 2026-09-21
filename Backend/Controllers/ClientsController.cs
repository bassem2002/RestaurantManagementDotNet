// Backend.Controllers/ClientsController.cs

using Backend.DTOs;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            var clients = await _clientRepository.GetClientsAsync();
            var result = clients.Select(c => new
            {
                c.Id,
                c.UserName,
                c.Email,
                c.FullName
            });
            return Ok(result);
        }

        [HttpGet("{userId}/cart")]
        public async Task<IActionResult> GetClientCart(string userId)
        {
            var cart = await _clientRepository.GetClientCartAsync(userId);
            if (cart == null || !cart.Items.Any())
                return Ok(new List<object>());

            var cartItems = cart.Items.Select(ci => new
            {
                ci.CartItemId,
                ci.ItemId,
                ItemName = ci.Item.Name,
                ci.Quantity,
                Price = ci.Item.Price,
                Total = ci.Quantity * ci.Item.Price
            });

            return Ok(cartItems);
        }

        // GET PROFILE
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var user = await _clientRepository.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.Phone,
                user.Address,
                user.City,
                user.PostalCode
            });
        }

        // UPDATE PROFILE
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var user = await _clientRepository.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            // Mise à jour uniquement si la valeur est fournie (non null)
            user.FullName = dto.FullName ?? user.FullName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Address = dto.Address ?? user.Address;
            user.City = dto.City ?? user.City;
            user.PostalCode = dto.PostalCode ?? user.PostalCode;

            var success = await _clientRepository.UpdateUserAsync(user);
            if (!success)
                return BadRequest("Failed to update user profile");

            return Ok(new
            {
                message = "Profile updated successfully",
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.Phone,
                user.Address,
                user.City,
                user.PostalCode
            });
        }
    }
}