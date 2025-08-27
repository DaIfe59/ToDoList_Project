using Microsoft.AspNetCore.Mvc;
using TODOListServer.Data;
using TODOListServer.Models;

namespace TODOListServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(AppDbContext context, ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/categories
        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            var categories = _context.Categories.ToList();
            _logger.LogInformation($"Загружено {categories.Count} категорий: {string.Join(", ", categories.Select(c => $"{c.Id}:{c.Name}"))}");
            return categories;
        }

        // POST: api/categories
        [HttpPost]
        public ActionResult<Category> AddCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Невалидная модель категории: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Добавление категории: Name={category.Name}");
            
            _context.Categories.Add(category);
            _context.SaveChanges();
            
            _logger.LogInformation($"Категория создана: ID={category.Id}, Name={category.Name}");
            return CreatedAtAction(nameof(GetCategories), new {id = category.Id}, category);
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();

            _logger.LogInformation($"Удаление категории: ID={id}, Name={category.Name}");
            
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
