using Microsoft.EntityFrameworkCore;
using Shop.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Shop.View
{
    public partial class NewSaleWindow : Window
    {
        private Sale _currentSale = new Sale();
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        public NewSaleWindow()
        {
            InitializeComponent();
            _currentSale.Date = DateTime.Today;
            DateTextBox.IsEnabled = false;
            CostTextBox.IsEnabled = false;

            using (var context = new ShopContext())
            {
                Products = new ObservableCollection<Product>(context.Products.ToList());
            }
            ProductNameComboBox.ItemsSource = Products;
            DataContext = _currentSale;

            ProductNameComboBox.SelectionChanged += ProductNameComboBox_SelectionChanged;
        }

        public ObservableCollection<Product> Products { get; set; }

        private void ProductNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProduct = (Product)ProductNameComboBox.SelectedItem;
            if (selectedProduct != null)
            {
                _currentSale.ProductId = selectedProduct.ProductId;
                _currentSale.Cost = selectedProduct.PriceUnit * _currentSale.AmountOfProducts;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSale.ProductId == 0)
            {
                MessageBox.Show("Выберите товар в ComboBox.", "Ошибка");
                return;
            }

            using (var context = new ShopContext())
            {
                var product = context.Products.Find(_currentSale.ProductId);
                if (product == null)
                {
                    MessageBox.Show("Товар с таким ID не найден.", "Ошибка");
                    return;
                }

                _currentSale.Cost = product.PriceUnit * _currentSale.AmountOfProducts;

                Sale newSale = new Sale
                {
                    ProductId = _currentSale.ProductId,
                    AmountOfProducts = _currentSale.AmountOfProducts,
                    Cost = _currentSale.Cost,
                    Date = _currentSale.Date,
                    SellerId = _currentSale.SellerId
                };

                context.Sales.Add(newSale);

                try
                {
                    context.SaveChanges();
                    DataChanged?.Invoke(this, EventArgs.Empty);
                    MessageBox.Show("Информация сохранена!", "Успешно");
                    this.Close();
                }
                catch (DbUpdateException ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка");
                    Console.WriteLine(ex);
                }
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e) { this.Close(); }
    }
}
