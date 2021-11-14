using System;
using DoToo.Repositories;
using DoToo.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace DoToo.ViewModels
{
    public class ItemViewModel : ViewModel
    {
        private TodoItemRepository repository;

        public ToDoItem Item { get; set; }

        public ItemViewModel(ToDoItem item) => Item = item;

        public event EventHandler ItemStatusChanged;

        public string StatusText => Item.Completed ? "Reactivate" : "Completed";

        public ItemViewModel(TodoItemRepository repository)
        {
            this.repository = repository;
            Item = new ToDoItem() { Due = DateTime.Now.AddDays(1) };
        }

        public ICommand Save => new Command(async () =>
        {
            await repository.AddOrUpdate(Item);
            await Navigation.PopAsync();
        });

        public ICommand ToggleCompleted => new Command((arg) =>
        {
            Item.Completed = !Item.Completed;
            ItemStatusChanged?.Invoke(this, new EventArgs());
        });
    }
}
