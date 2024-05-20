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

        private void back_admin_window(object sender, EventArgs e)
        {
            MainAdminWindow mainAdminWindow = new MainAdminWindow();
            this.Hide();
            mainAdminWindow.Show();
        }

        private void click_new_seller(object sender, EventArgs e)
        {
           
            NewSeller newSeller = new NewSeller();
            this.Hide();
            newSeller.Show();
        }

        public void Sell_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSeller = (Seller)Sell.SelectedItem;
            if (selectedSeller != null)
            {
                // Здесь можно использовать выбранного сотрудника для нужных действий
               EditSeller editSeller = new EditSeller(selectedSeller);
                editSeller.Show();
            }
        }



    }
}
