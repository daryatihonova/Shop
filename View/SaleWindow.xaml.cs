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

            using (var context = new ShopContext())
            {
                Sales = new ObservableCollection<Sale>(context.Sales
                    .Include(s => s.Product)
                    .Include(s => s.Seller)
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

        private void Click_Search(object sender, RoutedEventArgs e)
        {
            string date_Search = Date_Search.Text;
            bool isDateSearchEmpty = string.IsNullOrEmpty(date_Search);

            if (isDateSearchEmpty)
            {
                Sales = new ObservableCollection<Sale>(_sales);
            }
            else
            {
                using (var context = new ShopContext())
                {
                    var sales = context.Sales
                        .Include(s => s.Product)
                        .Include(s => s.Seller)
                        .AsQueryable();

                    if (!isDateSearchEmpty)
                    {
                        DateTime searchDate;
                        if (DateTime.TryParse(date_Search, out searchDate))
                        {
                            sales = sales.Where(sa => sa.Date.Date == searchDate.Date);
                        }
                        else
                        {
                            MessageBox.Show("Неверный формат даты. Введите дату в формате дд.ММ.гггг", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    var results = sales.ToList();

                    if (results.Count == 0)
                    {
                        MessageBox.Show("Продажи по указанным критериям не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Sales = new ObservableCollection<Sale>(results);
                    }
                }
            }
        }




    }
}
