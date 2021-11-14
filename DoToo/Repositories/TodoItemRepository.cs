using System;
using DoToo.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;

namespace DoToo.Repositories
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private SQLiteAsyncConnection connection;

        public event EventHandler<ToDoItem> OnItemAdded;

        public event EventHandler<ToDoItem> OnItemUpdated;

        public async Task<List<ToDoItem>> GetItems()
        {
            await CreateConnection();
            return await connection.Table<ToDoItem>().ToListAsync();
        }

        public async Task AddItem(ToDoItem item)
        {
            await CreateConnection();
            await connection.InsertAsync(item);
            OnItemAdded?.Invoke(this, item);
        }

        public async Task UpdateItem(ToDoItem item)
        {
            await CreateConnection();
            await connection.UpdateAsync(item);
            OnItemUpdated?.Invoke(this, item);
        }

        public async Task AddOrUpdate(ToDoItem item)
        {
            if (item.Id == 0)
            {
                await AddItem(item);
            }
            else
            {
                await UpdateItem(item);
            }
        }

        private async Task CreateConnection()
        {
            if (connection != null)
            {
                return;
            }

            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var databasePath = Path.Combine(documentPath, "TodoItems.db");

            connection = new SQLiteAsyncConnection(databasePath);

            await connection.CreateTableAsync<ToDoItem>();

            if (await connection.Table<ToDoItem>().CountAsync() == 0)
            {
                await connection.InsertAsync(new ToDoItem() { Title = "Welcome to DoToo", Due = DateTime.Now.AddDays(1) });
            }

        }
    }
}
