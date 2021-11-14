using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoToo.Models;

namespace DoToo.Repositories
{
    public interface ITodoItemRepository
    {
        event EventHandler<ToDoItem> OnItemAdded;
        event EventHandler<ToDoItem> OnItemUpdated;

        Task<List<ToDoItem>> GetItems();
        Task AddItem(ToDoItem item);
        Task UpdateItem(ToDoItem item);
        Task AddOrUpdate(ToDoItem item);
    }
}
