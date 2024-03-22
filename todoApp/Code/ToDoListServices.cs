using System.Collections.Generic;
using System.Threading.Tasks;
using todoApp.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace todoApp.Code
{
    public class ToDoListServices
    {
        private readonly TodoContext _context;
        private readonly AsymmetricHandler _asymmetricHandler; // Corrected the class name

        public ToDoListServices(TodoContext context, AsymmetricHandler asymmetricHandler)
        {
            _context = context;
            _asymmetricHandler = asymmetricHandler; // Corrected the field name
        }

        public async Task<List<TodoList>> GetTodoItemsByUserIdAsync(string userId)
        {
            // Directly return encrypted items; decryption will be handled in the Razor component
            return await _context.TodoLists.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task AddTodoItemAsync(TodoList newItem)
        {
            // Encrypt and add the new item
            newItem.Item = _asymmetricHandler.EncryptAsymtrisk(newItem.Item);
            _context.TodoLists.Add(newItem);

            // Save all changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTodoItemAsync(int itemId)
        {
            // Find the item to delete and remove it directly, without re-encrypting
            var itemToDelete = await _context.TodoLists.FindAsync(itemId);
            if (itemToDelete != null)
            {
                _context.TodoLists.Remove(itemToDelete);

                // Save all changes to the database
                await _context.SaveChangesAsync();
            }
        }

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