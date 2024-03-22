using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using todoApp.Models;
using System.Linq;

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
            var encryptedItems = await _context.TodoLists.Where(t => t.UserId == userId).ToListAsync();
            var decryptedItems = encryptedItems.Select(item =>
            {
                item.Item = _asymmetricHandler.DecryptAsymtrisk(item.Item); // Use the instance for decryption
                return item;
            }).ToList();

            return decryptedItems;
        }

        public async Task AddTodoItemAsync(TodoList newItem)
        {
            var todoItems = _context.TodoLists.Local.ToList();
            foreach (var item in todoItems)
            {
                item.Item = _asymmetricHandler.EncryptAsymtrisk(item.Item);
            }

            // Encrypt and add the new item
            newItem.Item = _asymmetricHandler.EncryptAsymtrisk(newItem.Item);
            _context.TodoLists.Add(newItem);

            // Now save all changes to the database
            await _context.SaveChangesAsync();
        }


        public async Task DeleteTodoItemAsync(int itemId)
        {
            // Re-encrypt all items in the local context before deleting
            var todoItems = _context.TodoLists.Local.ToList();
            foreach (var item in todoItems)
            {
                if (!string.IsNullOrEmpty(item.Item))
                {
                    item.Item = _asymmetricHandler.EncryptAsymtrisk(item.Item);
                }
            }

            // Find the item to delete and remove it
            var itemToDelete = await _context.TodoLists.FindAsync(itemId);
            if (itemToDelete != null)
            {
                _context.TodoLists.Remove(itemToDelete);

                // Now save all changes to the database
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