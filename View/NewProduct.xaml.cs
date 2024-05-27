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
            using (var context = new Model.ShopContext())
            {
                _currentProduct.Name = NameTextBox.Text;
                _currentProduct.Description = DescriptionTextBox.Text;
                _currentProduct.UnitOfMeasurement = UnitOfMeasurementTextBox.Text;
                _currentProduct.PriceUnit = decimal.Parse(PriceUnitTextBox.Text.Trim(), CultureInfo.InvariantCulture);
                _currentProduct.Quantity = int.Parse(QuantityTextBox.Text);
                _currentProduct.DateOfLastDelivery = DateTime.Today.Date;

                var existingProduct = context.Products.FirstOrDefault(p => p.Name == _currentProduct.Name);
               
                if (existingProduct != null)
                {
                    _currentProduct = existingProduct;
                    MessageBox.Show($"Такой товар уже существует. Перейдите в окно изменения. Код товара: {existingProduct.ProductId}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (_currentProduct.ProductId == 0)
                    {
                        decimal totalCost = _currentProduct.PriceUnit * _currentProduct.Quantity;
                        context.Products.Add(_currentProduct);
                        context.SaveChanges();
                        Model.Storage newStorageEntry = new Model.Storage
                        {
                            QuantityOfProducts = _currentProduct.Quantity,
                            TotalCost = totalCost,
                            DateDelivery = DateTime.Now,
                            ProductId = _currentProduct.ProductId,
                            Product = _currentProduct
                        };
                        context.Storages.Add(newStorageEntry);
                    }
                    else
                    {
                        var existingProductToUpdate = context.Products.Find(_currentProduct.ProductId);
                        if (existingProductToUpdate != null)
                        {
                            existingProductToUpdate.Name = _currentProduct.Name;
                            existingProductToUpdate.Description = _currentProduct.Description;
                            existingProductToUpdate.UnitOfMeasurement = _currentProduct.UnitOfMeasurement;
                            existingProductToUpdate.PriceUnit = _currentProduct.PriceUnit;
                            existingProductToUpdate.Quantity = _currentProduct.Quantity;
                            existingProductToUpdate.DateOfLastDelivery = _currentProduct.DateOfLastDelivery;
                            decimal totalCost = existingProductToUpdate.PriceUnit * existingProductToUpdate.Quantity;
                            var storageEntry = context.Storages.FirstOrDefault(s => s.ProductId == existingProductToUpdate.ProductId);
                            if (storageEntry != null)
                            {
                                storageEntry.QuantityOfProducts = existingProductToUpdate.Quantity;
                                storageEntry.TotalCost = totalCost;
                                storageEntry.DateDelivery = DateTime.Now;
                            }
                        }
                    }
                    context.SaveChanges();
                    DataContext = _currentProduct;
                    DataChanged?.Invoke(this, EventArgs.Empty);
                    MessageBox.Show("Информация сохранена!", "Успешно");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

            private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                var product = context.Products.FirstOrDefault(p => p.Name == NameTextBox.Text);
                if (product != null)
                {
                    MessageBox.Show($"Такой товар уже существует. Перейдите в окно изменения. ID товара: {product.ProductId}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }

                else Prod_Count.Text = "Новый товар";

            }
        }


                    
    }
}