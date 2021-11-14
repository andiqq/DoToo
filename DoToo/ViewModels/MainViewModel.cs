using System.Threading.Tasks;
using DoToo.Repositories;
using System.Windows.Input;
using DoToo.Views;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using DoToo.Models;
using System;

namespace DoToo.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public ObservableCollection<ItemViewModel> Items { get; set; }

        private readonly TodoItemRepository repository;


        public MainViewModel(TodoItemRepository repository)
        {
            repository.OnItemAdded += (sender, item) =>

            Items.Add(CreateTodoItemViewModel(item));
            repository.OnItemUpdated += (sender, item) =>
            Task.Run(async () => await LoadData());

            this.repository = repository;
            Task.Run(async () => await LoadData());
        }


        public ICommand AddItem => new Command(async () =>
        {
            var itemView = Resolver.Resolve<ItemView>();
            await Navigation.PushAsync(itemView);
        });


        private async Task LoadData()
        {
            var items = await repository.GetItems();

            if (!ShowAll)
            {
                items = items.Where(x => x.Completed == false).ToList();
            }

            var itemViewModels = items.Select(i => CreateTodoItemViewModel(i));
            Items = new ObservableCollection<ItemViewModel>(itemViewModels);
        }


        private ItemViewModel CreateTodoItemViewModel(ToDoItem item)
        {
            var itemViewModel = new ItemViewModel(item);

            itemViewModel.ItemStatusChanged += ItemStatusChanged;

            return itemViewModel;
        }


        public ItemViewModel SelectedItem
        {
            get { return null; }
            set
            {
                Device.BeginInvokeOnMainThread(async () => await
                NavigateToItem(value));
                RaisePropertyChanged(nameof(SelectedItem));
            }
        }


        private async Task NavigateToItem(ItemViewModel item)
        {
            if (item == null)
            {
                return;
            }

            var itemView = Resolver.Resolve<ItemView>();
            var vm = itemView.BindingContext as ItemViewModel;
            vm.Item = item.Item;

            await Navigation.PushAsync(itemView);
        }

        private void ItemStatusChanged(object sender, EventArgs e)
        {
            if (sender is ItemViewModel item)
            {
                if (!ShowAll && item.Item.Completed)
                {
                    Items.Remove(item);
                }

                Task.Run(async () => await repository.UpdateItem(item.Item));
            }
        }

        public string FilterText => ShowAll ? "All" : "Active";

        public ICommand ToggleFilter => new Command(async () =>
        {
            ShowAll = !ShowAll;
            await LoadData();
        });

        public bool ShowAll { get; set; }
    }
}
