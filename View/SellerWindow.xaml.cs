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
    /// Логика взаимодействия для SellerWindow.xaml
    /// </summary>
    public partial class SellerWindow : Window
    {
        private ObservableCollection<Seller> _sellers;

        public ObservableCollection<Seller> Sellers
        {
            get { return _sellers; }
            set { _sellers = value; Sell.ItemsSource = value; }
        }

        public SellerWindow()
        {
            InitializeComponent();

            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();

            using (var context = new ShopContext())
            {
                Sellers = new ObservableCollection<Seller>(context.Sellers.ToList());
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        }

        private void back_admin_window(object sender, RoutedEventArgs e)
        {
            MainAdminWindow mainAdminWindow = new MainAdminWindow();
            this.Hide();
            mainAdminWindow.Show();
        }

        private void click_new_seller(object sender, RoutedEventArgs e)
        {
            NewSeller newSeller = new NewSeller(null);
            newSeller.DataChanged += UpdateDataGrid;

            newSeller.Show();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            NewSeller editWindow = new NewSeller((sender as Button).DataContext as Seller);
            editWindow.DataChanged += UpdateDataGrid;

            editWindow.ShowDialog();
        }

        private void click_delete_seller(Object sender, RoutedEventArgs e)
        {
            var sellersForRemoving = Sell.SelectedItems.Cast<Seller>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {sellersForRemoving.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new ShopContext())
                    {
                        context.Sellers.RemoveRange(sellersForRemoving);
                        context.SaveChanges();
                        MessageBox.Show("Данные удалены");
                        Sellers = new ObservableCollection<Seller>(context.Sellers.ToList());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void UpdateDataGrid(object sender, EventArgs e)
        {
            // Обновление данных в таблице
            using (var context = new ShopContext())
            {
                Sellers = new ObservableCollection<Seller>(context.Sellers.ToList());
            }
        }
    }
}
