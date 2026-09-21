using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartRepository repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        // Admin only
        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            return Ok(await _repo.GetAllCartsAsync());
        }

        // User cart
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            try
            {
                var cart = await _repo.GetCartByUserIdAsync(userId)
                           ?? await _repo.CreateCartAsync(userId);

                var user = await _userManager.FindByIdAsync(userId);
                var username = user?.UserName ?? user?.FullName ?? string.Empty;

                return Ok(new { Cart = cart, UserName = username });
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{userId}/add/{itemId}")]
        public async Task<IActionResult> AddItem(string userId, int itemId, [FromBody] int quantity)
        {
            try
            {
                var cart = await _repo.AddItemAsync(userId, itemId, quantity);
                var user = await _userManager.FindByIdAsync(userId);
                var username = user?.UserName ?? user?.FullName ?? string.Empty;
                return Ok(new { Cart = cart, UserName = username });
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new { Message = knf.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{userId}/remove/{itemId}")]
        public async Task<IActionResult> RemoveItem(string userId, int itemId)
        {
            try
            {
                var success = await _repo.RemoveItemAsync(userId, itemId);
                var user = await _userManager.FindByIdAsync(userId);
                var username = user?.UserName ?? user?.FullName ?? string.Empty;
                return Ok(new { Success = success, UserName = username });
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new { Message = knf.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{userId}/clear")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            try
            {
                var success = await _repo.ClearCartAsync(userId);
                var user = await _userManager.FindByIdAsync(userId);
                var username = user?.UserName ?? user?.FullName ?? string.Empty;
                return Ok(new { Success = success, UserName = username });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
