using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TODOListServer.Data;
using TODOListServer.Models;

namespace TODOListServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TasksController> _logger;

        public TasksController(AppDbContext context, ILogger<TasksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/tasks
        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> GetTasks()
        {
            var tasks = _context.Tasks.Include(t => t.Category).ToList();
            _logger.LogInformation($"Загружено {tasks.Count} задач. Категории: {string.Join(", ", tasks.Select(t => t.Category?.Name ?? "null"))}");
            return tasks;
        }

        // GET: api/tasks/debug - для отладки
        [HttpGet("debug")]
        public ActionResult<object> DebugDatabase()
        {
            var tasks = _context.Tasks.ToList();
            var categories = _context.Categories.ToList();
            
            var debugInfo = new
            {
                Tasks = tasks.Select(t => new { t.Id, t.Title, t.CategoryId }),
                Categories = categories.Select(c => new { c.Id, c.Name }),
                TaskCount = tasks.Count,
                CategoryCount = categories.Count
            };
            
            _logger.LogInformation($"Debug: {debugInfo.TaskCount} задач, {debugInfo.CategoryCount} категорий");
            return debugInfo;
        }

        // POST: api/tasks/reset - для сброса и пересоздания тестовых данных
        [HttpPost("reset")]
        public ActionResult<object> ResetDatabase()
        {
            try
            {
                // Удаляем все задачи и категории
                _context.Tasks.RemoveRange(_context.Tasks);
                _context.Categories.RemoveRange(_context.Categories);
                _context.SaveChanges();
                
                // Создаем тестовые категории
                var categories = new List<Category>
                {
                    new Category { Name = "Работа" },
                    new Category { Name = "Личное" },
                    new Category { Name = "Покупки" },
                    new Category { Name = "Здоровье" }
                };
                
                _context.Categories.AddRange(categories);
                _context.SaveChanges();
                
                // Создаем тестовые задачи
                var tasks = new List<TaskItem>
                {
                    new TaskItem { Title = "Тестовая задача 1", CategoryId = categories[0].Id },
                    new TaskItem { Title = "Тестовая задача 2", CategoryId = categories[1].Id },
                    new TaskItem { Title = "Тестовая задача 3", CategoryId = categories[2].Id }
                };
                
                _context.Tasks.AddRange(tasks);
                _context.SaveChanges();
                
                _logger.LogInformation("База данных сброшена и заполнена тестовыми данными");
                
                return new { Message = "База данных сброшена", TaskCount = tasks.Count, CategoryCount = categories.Count };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при сбросе базы данных: {ex.Message}");
                return BadRequest($"Ошибка при сбросе: {ex.Message}");
            }
        }

        // POST: api/tasks
        [HttpPost]
        public ActionResult<TaskItem> AddTask([FromBody] TaskItem task)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Невалидная модель: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Добавление задачи: Title={task.Title}, CategoryId={task.CategoryId}");
            
            // Убеждаемся, что Category не передается в JSON
            task.Category = null;
            
            _context.Tasks.Add(task);
            _context.SaveChanges();
            
            // Загружаем категорию для возврата
            var createdTask = _context.Tasks.Include(t => t.Category).First(t => t.Id == task.Id);
            _logger.LogInformation($"Задача создана: ID={createdTask.Id}, CategoryId={createdTask.CategoryId}, Category={createdTask.Category?.Name ?? "null"}");
            
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, createdTask);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskItem updatedTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = _context.Tasks.Find(id);
            if (task == null) return NotFound();

            task.Title = updatedTask.Title;
            task.CategoryId = updatedTask.CategoryId;
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
