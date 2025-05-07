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
        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tasks
        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> GetTasks()
        {
            return _context.Tasks.Include(t=> t.Category).ToList();
        }

        // POST: api/tasks
        [HttpPost]
        public ActionResult<TaskItem> AddTask([FromBody] TaskItem task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTasks), new {id = task.Id},task);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskItem updatedTask)
        {
            var task= _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();

            task.Title = updatedTask.Title;
            task.CategoryId=updatedTask.CategoryId;
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/tasks/{id}
        public IActionResult DeleteTask(int id) 
        {
            var task =_context.Tasks.FirstOrDefault(c =>c.Id == id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
