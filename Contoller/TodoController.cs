using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using part2_exersice.DataBase;
using part2_exersice.Model.API_Models;

namespace part2_exersice.Contoller
{   
    [Authorize]
    [ApiController]
    [Route("todos")]
    public class TodoController : ControllerBase
    {
        private readonly DataBasePart2 _context;

        public TodoController(DataBasePart2 context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTodos()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var todos = _context.Todos
                .Include(t => t.Tags)
                .Where(t => t.UserId.ToString() == userId)
                .ToList();
            return Ok(todos);
        }

        [HttpPost]
        public IActionResult CreateTodo([FromBody] CreateTodo createTodo)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var newTodo = new Model.Todo
            {
                Name = createTodo.Name,
                UserId = int.Parse(userId)
            };

            _context.Todos.Add(newTodo);
            _context.SaveChanges();

            return Ok(new { message = "Todo created successfully", todo = newTodo });
        }

        [HttpGet("{id}")]
        public IActionResult GetTodoById(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var todo = _context.Todos
                .Include(t => t.Tags)
                .FirstOrDefault(t => t.Id == id && t.UserId.ToString() == userId);

            if (todo == null)
            {
                return NotFound(new { message = "Todo not found" });
            }

            return Ok(todo);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTodo(int id, [FromBody] UpdateTodo updateTodo)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var todo = _context.Todos
                .Include(t => t.Tags)
                .FirstOrDefault(t => t.Id == id && t.UserId.ToString() == userId);

            if (todo == null)
            {
                return NotFound(new { message = "Todo not found" });
            }

            todo.Name = updateTodo.Name;

            _context.SaveChanges();

            return Ok(new { message = "Todo updated successfully", todo });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTodo(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var todo = _context.Todos
                .FirstOrDefault(t => t.Id == id && t.UserId.ToString() == userId);

            if (todo == null)
            {
                return NotFound(new { message = "Todo not found" });
            }

            _context.Todos.Remove(todo);
            _context.SaveChanges();

            return Ok(new { message = "Todo deleted successfully" });
        }
    }
}