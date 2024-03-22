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

        public async Task<List<TodoList>> GetEncryptedTodoItemsByUserIdAsync(string userId)
        {
            // Fetches only encrypted items, direct from the database
            return await _context.TodoLists.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<List<TodoItemDto>> GetDecryptedTodoItemsByUserIdAsync(string userId)
        {
            // Fetches encrypted items and then maps them to DTOs with decrypted content
            var encryptedItems = await GetEncryptedTodoItemsByUserIdAsync(userId);
            return encryptedItems.Select(item => new TodoItemDto
            {
                Id = item.Id,
                UserId = item.UserId,
                Item = _asymmetricHandler.DecryptAsymtrisk(item.Item)
            }).ToList();
        }

        public async Task AddTodoItemAsync(TodoItemDto newItemDto)
        {
            var newItem = new TodoList
            {
                UserId = newItemDto.UserId,
                Item = _asymmetricHandler.EncryptAsymtrisk(newItemDto.Item)
            };

            _context.TodoLists.Add(newItem);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteTodoItemAsync(int itemId)
        {
            // Directly find the item to delete without modifying other items
            var itemToDelete = await _context.TodoLists.FindAsync(itemId);
            if (itemToDelete != null)
            {
                _context.TodoLists.Remove(itemToDelete);
                // Save the change to the database
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