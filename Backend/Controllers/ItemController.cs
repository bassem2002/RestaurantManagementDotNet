using Backend.DTOs;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _repo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IWebHostEnvironment _env;

        public ItemController(
            IItemRepository repo,
            ICategoryRepository categoryRepo,
            IWebHostEnvironment env)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
            _env = env;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repo.GetAllAsync();

            var dtoList = items.Select(i => new ItemDTO
            {
                ItemId = i.ItemId,
                Name = i.Name,
                Price = i.Price,
                CategoryId = i.CategoryId,
                CategoryName = i.Category?.Name,
                PhotoUrl = i.PhotoPath
            });

            return Ok(dtoList);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            var dto = new ItemDTO
            {
                ItemId = item.ItemId,
                Name = item.Name,
                Price = item.Price,
                CategoryId = item.CategoryId,
                CategoryName = item.Category?.Name
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ItemCreateUpdateDTO itemDto)
        {
            var category = await _categoryRepo.GetByIdAsync(itemDto.CategoryId);
            if (category == null)
                return BadRequest("Invalid category.");

            string? photoPath = null;

            if (itemDto.Photo != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "items");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(itemDto.Photo.FileName)}";
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await itemDto.Photo.CopyToAsync(stream);

                photoPath = $"/uploads/items/{fileName}";
            }

            var item = new Item
            {
                Name = itemDto.Name,
                Price = (decimal)itemDto.Price,
                CategoryId = itemDto.CategoryId,
                PhotoPath = photoPath
            };

            var created = await _repo.AddAsync(item);

            return CreatedAtAction(nameof(Get), new { id = created.ItemId }, new ItemDTO
            {
                ItemId = created.ItemId,
                Name = created.Name,
                Price = created.Price,
                CategoryId = created.CategoryId,
                CategoryName = created.Category?.Name,
                PhotoUrl = created.PhotoPath
            });
        }


        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ItemCreateUpdateDTO itemDto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var category = await _categoryRepo.GetByIdAsync(itemDto.CategoryId);
            if (category == null)
                return BadRequest($"Category with ID {itemDto.CategoryId} does not exist.");

            existing.Name = itemDto.Name;
            existing.Price = (decimal)itemDto.Price;
            existing.CategoryId = itemDto.CategoryId;

            var updated = await _repo.UpdateAsync(existing);

            var dto = new ItemDTO
            {
                ItemId = updated.ItemId,
                Name = updated.Name,
                Price = updated.Price,
                CategoryId = updated.CategoryId,
                CategoryName = updated.Category?.Name
            };

            return Ok(dto);
        }


        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}