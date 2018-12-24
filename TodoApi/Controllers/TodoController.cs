using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    // The ApiController attribute indicates taht the controller respons to web API requests.
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TodoApi.Controllers.TodoController"/> class.
        /// If all items are deleted, this constructor will create Item1 again
        /// </summary>
        /// <param name="context">Context.</param>
        /// <remarks>
        /// Use DI to inject hte database context TodoContext into the controller.
        /// The database context is uses in each of the CRUD methods in the controller
        /// </remarks>
        public TodoController(TodoContext context)
        {
            this._context = context;

            if (this._context.TodoItems.Count() == 0)
            {
                // Create a new TodoItem if collection is empty, which means you can't delete all TodoItems
                this._context.TodoItems.Add(new TodoItem { Name = "Item1" });
                this._context.SaveChanges();
            }
        }

        // GET: api/Todo
        // The HttpGet attribute denotes a method that respons to an HTTP Get request
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await this._context.TodoItems.ToListAsync();
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await this._context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                // return a 404 NotFound error code
                return NotFound();
            }

            return todoItem;
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            this._context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            // Use the GetTodo Named Route
            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // Put: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            // According to the HTTP specificaiton, a PUT request requires the client ot send the entire updated entity, not just the changes
            this._context.Entry(todoItem).State = EntityState.Modified;
            await this._context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await this._context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            this._context.TodoItems.Remove(todoItem);
            await this._context.SaveChangesAsync();

            return todoItem;
        }
    }
}
