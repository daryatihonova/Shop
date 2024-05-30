using Shop.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Shop.View
{
    public partial class NewProduct : Window
    {
        private Model.Product _currentProduct = new Product();

        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        public NewProduct(Product selectedProduct)
        {
            InitializeComponent();

            if (selectedProduct != null)
                _currentProduct = selectedProduct;

            _currentProduct.DateOfLastDelivery = DateTime.Today;
            DataContext = _currentProduct;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(UnitOfMeasurementTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceUnitTextBox.Text) ||
                string.IsNullOrWhiteSpace(QuantityTextBox.Text))
            {
                MessageBox.Show("Необходимо заполнить все поля!", "Ошибка");
                return;
            }

            using (var context = new Model.ShopContext())
            {
                var existingProduct = context.Products.FirstOrDefault(p => p.Name == _currentProduct.Name);

                if (existingProduct != null)
                {
                    if (_currentProduct.ProductId == 0)
                    {
                        MessageBox.Show($"Товар с названием {_currentProduct.Name} уже существует.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        context.Entry(existingProduct).CurrentValues.SetValues(_currentProduct);
                        context.SaveChanges();
                        MessageBox.Show($"Товар успешно изменен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    context.Products.Add(_currentProduct);
                    context.SaveChanges();

                    // Добавление товара на склад
                    var newStorageItem = new Storage
                    {
                        ProductId = _currentProduct.ProductId,
                        QuantityOfProducts = _currentProduct.Quantity,
                        TotalCost = _currentProduct.PriceUnit * _currentProduct.Quantity,
                        DateDelivery = DateTime.Today
                    };
                    context.Storages.Add(newStorageItem);
                    context.SaveChanges();

                    MessageBox.Show($"Новый товар успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            DataChanged?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                var product = context.Products.FirstOrDefault(p => p.Name == NameTextBox.Text);

                if (product != null)
                {
                    MessageBox.Show($"Такой товар уже существует. Перейдите в окно изменения. ID товара: {product.ProductId}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
                else
                {
                    Prod_Count.Text = "Новый товар";
                }
            }
        }
    }
}
