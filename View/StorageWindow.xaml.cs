using Microsoft.EntityFrameworkCore;
using Shop.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shop.View
{
    /// <summary>
    /// Логика взаимодействия для StorageWindow.xaml
    /// </summary>
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
                Storages = new ObservableCollection<Storage>(context.Storages.ToList());
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

        private void click_storage(object sender, RoutedEventArgs e)
        {
            ChangeStorage  changeStorage = new ChangeStorage();
            this.Hide();
            changeStorage.Show();
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
                        .Where(st => EF.Functions.Like(st.ProductId.ToString(), $"%{stor_Search}%"))
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

    }
}
