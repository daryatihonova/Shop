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
    /// Логика взаимодействия для SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        private ObservableCollection<Sale> _sales;
        public ObservableCollection<Sale> Sales 
        {
            get { return _sales; }
            set { _sales = value; Sal.ItemsSource = value; }
        }
        public SaleWindow()
        {
            InitializeComponent();
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();

            using(var context = new ShopContext())
            {
                Sales = new ObservableCollection<Sale>(context.Sales.ToList());
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

        private void Click_Search(object sender, RoutedEventArgs e)
        {
            string date_Search = Date_Search.Text;
            string seller_Search = Seller_Search.Text;
            bool isDateSearchEmpty = string.IsNullOrEmpty(date_Search);
            bool isSellerSearchEmpty = string.IsNullOrEmpty(seller_Search);

            if (isDateSearchEmpty && isSellerSearchEmpty)
            {
                // Оба поля поиска пусты, отобразить все продажи
                Sales = new ObservableCollection<Sale>(_sales);
            }
            else
            {
                using (var context = new ShopContext())
                {
                    var sales = context.Sales.AsQueryable(); // Создаем запрос

                    // Добавляем условия поиска в зависимости от заполненных полей
                    if (!isDateSearchEmpty)
                    {
                        sales = sales.Where(sa => EF.Functions.Like(sa.Date.ToString(), $"%{date_Search}%"));
                    }
                    if (!isSellerSearchEmpty)
                    {
                        sales = sales.Where(sa => EF.Functions.Like(sa.SellerId.ToString(), $"%{seller_Search}%"));
                    }

                    var results = sales.ToList(); // Выполняем запрос и получаем результаты

                    if (results.Count == 0)
                    {
                        // Ничего не найдено, отобразить сообщение об ошибке
                        MessageBox.Show("Продажи по указанным критериям не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        // Найдены продажи, обновить список
                        Sales = new ObservableCollection<Sale>(results);
                    }
                }
            }
        }

    }
}
