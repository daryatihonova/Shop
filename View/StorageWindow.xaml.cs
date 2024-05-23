using Microsoft.EntityFrameworkCore;
using Shop.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Shop.View
{
    public partial class StorageWindow : Window
    {
        private ObservableCollection<Storage> _storages;

        public ObservableCollection<Storage> Storages
        {
            get { return _storages; }
            set { _storages = value; Stor.ItemsSource = value; }
        }

        public StorageWindow()
        {
            InitializeComponent();

            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();

            using (var context = new ShopContext())
            {
                Storages = new ObservableCollection<Storage>(context.Storages
                    .Include(s => s.Product) // Add this line to include the Product objects
                    .ToList());
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        }

        private void back_admin_window(object sender, EventArgs e)
        {
            MainAdminWindow mainAdminWindow = new MainAdminWindow();
            this.Hide();
            mainAdminWindow.Show();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeStorage changeStorage = new ChangeStorage((sender as Button).DataContext as Storage);
            changeStorage.DataChanged += UpdateDataGrid;
            changeStorage.ShowDialog();
        }

        private void Click_Stor_Search(object sender, RoutedEventArgs e)
        {
            string stor_Search = Stor_Search.Text;
            if (string.IsNullOrEmpty(stor_Search))
            {
                Storages = new ObservableCollection<Storage>(_storages);
            }
            else
            {
                using (var context = new ShopContext())
                {
                    var storages = context.Storages
                        .Include(s => s.Product)
                        .Where(s => EF.Functions.Like(s.Product.Name, $"%{stor_Search}%"))
                        .ToList();
                    if (storages.Count == 0)
                    {
                        MessageBox.Show("Товар на складе не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Storages = new ObservableCollection<Storage>(storages);
                    }
                }
            }
        }


        private void click_delete_storage(object sender, RoutedEventArgs e)
        {
            var storagesForRemoving = Stor.SelectedItems.Cast<Storage>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить следующие {storagesForRemoving.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new ShopContext())
                    {
                        context.Storages.RemoveRange(storagesForRemoving);
                        context.SaveChanges();
                        MessageBox.Show("Данные удалены");
                        Storages = new ObservableCollection<Storage>(context.Storages.ToList());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void UpdateDataGrid(object sender, RoutedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                Storages = new ObservableCollection<Storage>(context.Storages.ToList());
            }
        }
    }
}
