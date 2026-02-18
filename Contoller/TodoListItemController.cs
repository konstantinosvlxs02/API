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
    public class TodoListItemController : ControllerBase
    {
        private readonly DataBasePart2 _context;

        public TodoListItemController(DataBasePart2 context)
        {
            _context = context;
        }

        [HttpGet("{id}/items/{itemId}")]
        public IActionResult GetTodoListItem(int id, int itemId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var todoListItem = _context.TodoListItems
                .Include(t => t.Todo)
                .FirstOrDefault(t => t.Id == itemId && t.TodoId == id && t.Todo!.UserId.ToString() == userId);
            if (todoListItem == null)
            {
                return NotFound(new { message = "Todo list item not found" });
            }
            return Ok(todoListItem);
        }

        [HttpPost("{id}/items")]
        public IActionResult CreateTodoListItem(int id, [FromBody] TodoItemCreate createTodoListItem)
        {   

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var todo = _context.Todos.FirstOrDefault(t => t.Id == id && t.UserId.ToString() == userId);
            if (todo == null)
            {
                return NotFound(new { message = "Todo not found" });
            }

            var newTodoListItem = new Model.TodoListItem
            {
                Content = createTodoListItem.Content,
                TodoId = id
            };

            _context.TodoListItems.Add(newTodoListItem);
            _context.SaveChanges();
            var updatedTodo = _context.Todos
                .Include(t => t.Tags)
                .FirstOrDefault(t => t.Id == id);

            return Ok(new { message = "Todo item created successfully", todoListItem = newTodoListItem, todo = updatedTodo });
        }

        [HttpPut("{id}/items/{itemId}")]
        public IActionResult UpdateTodoListItem(int id, int itemId, [FromBody] TodoItemUpdate todoItemUpdate)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var todoListItem = _context.TodoListItems
                .Include(t => t.Todo)
                .FirstOrDefault(t => t.Id == itemId && t.TodoId == id && t.Todo!.UserId.ToString() == userId);
                
            if (todoListItem == null)
            {
                return NotFound(new { message = "Todo list item not found" });
            }

            todoListItem.Content = todoItemUpdate.Content;
            todoListItem.IsCompleted = todoItemUpdate.IsCompleted;

            _context.SaveChanges();

            return Ok(new { message = "Todo list item updated successfully", todoListItem });
        }

        [HttpDelete("{id}/items/{itemId}")]
        public IActionResult DeleteTodoListItem(int id, int itemId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var todoListItem = _context.TodoListItems
                .Include(t => t.Todo)
                .FirstOrDefault(t => t.Id == itemId && t.TodoId == id && t.Todo!.UserId.ToString() == userId);
                
            if (todoListItem == null)
            {
                return NotFound(new { message = "Todo item not found" });
            }

            _context.TodoListItems.Remove(todoListItem);
            _context.SaveChanges();

            return Ok(new { message = "Todo item deleted successfully" });
        }
    }
}