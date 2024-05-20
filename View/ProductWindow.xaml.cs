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
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        { 
            get { return _products; }
            set { _products = value; Prod.ItemsSource = value; }
        }
        public ProductWindow()
        {
            InitializeComponent();
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();

            using (var context = new ShopContext())
            {
                Products = new ObservableCollection<Product>(context.Products.ToList());
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

        private void click_new_product(object sender, EventArgs e)
        {
            NewProduct newProduct = new NewProduct();
            this.Hide();
            newProduct.Show();
        }

        private void Click_Prod_Search(object sender, EventArgs e)
        {
            string prod_Search = Prod_Search.Text;
            if (string.IsNullOrEmpty(prod_Search))
            {
                Products = new ObservableCollection<Product>(_products);
            }
            else
            {
                using (var context = new ShopContext())
                {
                    var products = context.Products.Where(p => EF.Functions.Like(p.Name, $"%{prod_Search}%")).ToList();
                    if (products.Count == 0)
                    {
                        MessageBox.Show("Такого товара не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Products = new ObservableCollection<Product> (products);
                    }
                }
            }
        }


    }
}
