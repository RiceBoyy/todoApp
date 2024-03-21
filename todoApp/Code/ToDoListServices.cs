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
            // Assuming 'cprNr' is correctly mapped to 'UserId' in the context of your application
            return await _context.TodoLists
                                 .Where(t => t.UserId == cprNr)
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

        public async Task DeleteAllTodoItemsByUserIdAsync(int userId)
        {
            // Retrieve all todo items for the specified user
            var items = await _context.TodoLists
                                      .Where(t => t.UserId == userId)
                                      .ToListAsync();

            // Remove all retrieved items
            _context.TodoLists.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAllTodoItemsAsync()
        {
            // Retrieve all todo items
            var allItems = await _context.TodoLists.ToListAsync();

            // Remove all items
            _context.TodoLists.RemoveRange(allItems);
            await _context.SaveChangesAsync();
        }

    }
}
