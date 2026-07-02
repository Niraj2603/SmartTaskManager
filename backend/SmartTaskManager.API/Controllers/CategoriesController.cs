using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTaskManager.API.Data;
using SmartTaskManager.API.DTOs;
using SmartTaskManager.API.Models;

namespace SmartTaskManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _db.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryDto dto)
        {
            if (await _db.Categories.AnyAsync(c => c.CategoryName == dto.CategoryName))
                return BadRequest(new { message = "Category already exists." });

            var category = new Category { CategoryName = dto.CategoryName };
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return Ok(category);
        }
    }
}
