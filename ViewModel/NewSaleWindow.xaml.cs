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
        private int _currentUserId;

        public NewSaleWindow(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
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
                MessageBox.Show("Выберите товар.", "Ошибка");
                return;
            }

            using (var context = new ShopContext())
            {
                var storage = context.Storages.Include(s => s.Product).FirstOrDefault(s => s.ProductId == _currentSale.ProductId);
                if (storage == null)
                {
                    MessageBox.Show("Товар не найден на складе.", "Ошибка");
                    return;
                }

                // Обновление количества товара на складе в текстовом блоке
                ProductQuantityTextBlock.Text = storage.QuantityOfProducts.ToString();

                if (storage.QuantityOfProducts < _currentSale.AmountOfProducts)
                {
                    MessageBox.Show("Недостаточно товара на складе.", "Ошибка");
                    return;
                }

                decimal cost = storage.Product.PriceUnit * _currentSale.AmountOfProducts;

                Sale newSale = new Sale
                {
                    ProductId = _currentSale.ProductId,
                    AmountOfProducts = _currentSale.AmountOfProducts,
                    Cost = cost,
                    Date = _currentSale.Date,
                    SellerId = _currentUserId
                };

                storage.QuantityOfProducts -= _currentSale.AmountOfProducts;
                storage.TotalCost -= cost;

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


        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductNameComboBox.SelectedItem != null)
            {
                Product selectedProduct = (Product)ProductNameComboBox.SelectedItem;

                using (var context = new ShopContext())
                {
                    var storage = context.Storages.FirstOrDefault(s => s.ProductId == selectedProduct.ProductId);
                    if (storage != null)
                    {
                        ProductQuantityTextBlock.Text = $"Количество товаров на складе: {storage.QuantityOfProducts}";
                    }
                }
            }
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e) 
        { this.Close(); }
    }
}
