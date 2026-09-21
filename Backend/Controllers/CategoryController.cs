using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _repo;

        public CategoryController(ICategoryRepository repo)
        {
            _repo = repo;
        }

        // ------------------- GET /api/Category -------------------
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repo.GetAllAsync();
            return Ok(categories);
        }

        // ------------------- GET /api/Category/{id} -------------------
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // ------------------- POST /api/Category -------------------
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            if (category == null || string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Category name is required");

            var created = await _repo.AddAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
        }

        // ------------------- PUT /api/Category/{id} -------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            var updated = await _repo.UpdateAsync(id, category);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // ------------------- DELETE /api/Category/{id} -------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
