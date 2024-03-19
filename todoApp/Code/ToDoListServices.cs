using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using todoApp.Models;

namespace todoApp.Code
{
    public class ToDoListServices
    {
        private readonly TodoContext _context;

        public ToDoListServices(TodoContext context)
        {
            _context = context;
        }

        public async Task<List<TodoList>> GetTodoItemsByCPRNrAsync(int cprNr)
        {
            return await _context.TodoLists
                                 .Where(t => t.UserId == cprNr) // Now correctly using UserId
                                 .ToListAsync();
        }

        public async Task AddTodoItemAsync(TodoList item)
        {
            _context.TodoLists.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTodoItemAsync(int itemId)
        {
            var item = await _context.TodoLists.FindAsync(itemId);
            if (item != null)
            {
                _context.TodoLists.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }

}
