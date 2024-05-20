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
    /// Логика взаимодействия для SaleWindowForSeller.xaml
    /// </summary>
    public partial class SaleWindowForSeller : Window
    {
        private ObservableCollection<Sale> _sales;
        public ObservableCollection<Sale> Sales
        {
            get { return _sales; }
            set { _sales = value; Sal.ItemsSource = value; }
        }
        public SaleWindowForSeller()
        {
            InitializeComponent();
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();

            using (var context = new ShopContext())
            {
                Sales = new ObservableCollection<Sale>(context.Sales.ToList());
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        }

        private void back(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Hide();
            mainWindow.Show();
        }
        private void click_new_sale(object sender, RoutedEventArgs e)
        {
            NewSaleWindow newSaleWindow = new NewSaleWindow();
            this.Hide();
            newSaleWindow.Show();
        }

        private void Click_Prod_Search(object sender, RoutedEventArgs e)
        {
            string prod_Search = Prod_Search.Text;
            if (string.IsNullOrEmpty(prod_Search))
            {
                Sales = new ObservableCollection<Sale>(_sales);
            }
            else
            {
                using (var context = new ShopContext())
                {
                    var sales = context.Sales
                       .Where(st => EF.Functions.Like(st.ProductId.ToString(), $"%{prod_Search}%"))
                       .ToList();

                    if (sales.Count == 0)
                    {
                        MessageBox.Show("Продажа такого товара не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Sales = new ObservableCollection<Sale>(sales);
                    }
                }
            }
        }
    }
}
