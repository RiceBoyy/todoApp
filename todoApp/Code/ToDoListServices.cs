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

        public async Task<List<TodoList>> GetTodoItemsByUserIdAsync(string userId)
        {
            // Fetch TodoList items filtered by string UserId
            return await _context.TodoLists.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task AddTodoItemAsync(TodoList item)
        {
            _context.TodoLists.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTodoItemAsync(int itemId)
        {
            // Assuming itemId is the primary key and of type int. If it's a string, change the method parameter type.
            var item = await _context.TodoLists.FindAsync(itemId);
            if (item != null)
            {
                _context.TodoLists.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        // This method suggests it should filter deletions by userId, but currently, it does not take userId into account.
        // Assuming you want to delete all items for a given userId:
        public async Task DeleteAllTodoItemsByUserIdAsync(string userId)
        {
            var items = await _context.TodoLists.Where(t => t.UserId == userId).ToListAsync();
            _context.TodoLists.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllTodoItemsAsync()
        {
            var allItems = await _context.TodoLists.ToListAsync();
            _context.TodoLists.RemoveRange(allItems);
            await _context.SaveChangesAsync();
        }
    }
}
