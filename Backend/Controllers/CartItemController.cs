using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemRepository _repository;

        public CartItemController(ICartItemRepository repository)
        {
            _repository = repository;
        }

        // Manual mapping method
        private CartItemDto ToReadDto(CartItem ci)
        {
            return new CartItemDto
            {
                CartItemId = ci.CartItemId,
                ItemId = ci.ItemId,
                //ItemName = ci.Item?.Name,
                Quantity = ci.Quantity
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repository.GetAllAsync();
            var dtos = items.Select(ToReadDto);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cartItem = await _repository.GetByIdAsync(id);
            if (cartItem == null) return NotFound();

            return Ok(ToReadDto(cartItem));
        }

        [HttpGet("cart/{cartId}")]
        public async Task<IActionResult> GetByCartId(int cartId)
        {
            var items = await _repository.GetByCartIdAsync(cartId);
            var dtos = items.Select(ToReadDto);

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CartItemCreateUpdateDto dto)
        {
            var entity = new CartItem
            {
                CartId = dto.CartId,
                ItemId = dto.ItemId,
                Quantity = dto.Quantity
            };

            var created = await _repository.AddAsync(entity);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.CartItemId },
                ToReadDto(created)
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CartItemCreateUpdateDto dto)
        {
            var updatedEntity = new CartItem
            {
                CartId = dto.CartId,
                ItemId = dto.ItemId,
                Quantity = dto.Quantity
            };

            var updated = await _repository.UpdateAsync(id, updatedEntity);

            if (updated == null)
                return NotFound();

            return Ok(ToReadDto(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
