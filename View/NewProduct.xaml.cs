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
            _currentProduct.DateOfLastDelivery = DateTime.Today; // Устанавливаем сегодняшнюю дату
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

                    if (_currentProduct.ProductId == 0)
                    {
                        decimal totalCost = _currentProduct.PriceUnit * _currentProduct.Quantity;
                        context.Products.Add(_currentProduct);
                        context.SaveChanges(); // Сохраняем изменения, чтобы получить ProductId
                        Model.Storage newStorageEntry = new Model.Storage
                        {
                            QuantityOfProducts = _currentProduct.Quantity,
                            TotalCost = totalCost,
                            DateDelivery = DateTime.Now,
                            ProductId = _currentProduct.ProductId,
                            Product = _currentProduct // Устанавливаем Product для Storage

                        };
                        context.Storages.Add(newStorageEntry);
                    }
                    else
                    {
                        var existingProduct = context.Products.Find(_currentProduct.ProductId);
                        if (existingProduct != null)
                        {
                            existingProduct.Name = _currentProduct.Name;
                            existingProduct.Description = _currentProduct.Description;
                            existingProduct.UnitOfMeasurement = _currentProduct.UnitOfMeasurement;
                            existingProduct.PriceUnit = _currentProduct.PriceUnit;
                            existingProduct.Quantity = _currentProduct.Quantity;
                            existingProduct.DateOfLastDelivery = _currentProduct.DateOfLastDelivery;
                            decimal totalCost = existingProduct.PriceUnit * existingProduct.Quantity;
                            var storageEntry = context.Storages.FirstOrDefault(s => s.ProductId == existingProduct.ProductId);
                            if (storageEntry != null)
                            {
                                storageEntry.QuantityOfProducts = existingProduct.Quantity;
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                var product = context.Products.FirstOrDefault(p => p.Name == NameTextBox.Text);
                if (product == null)
                {
                    Prod_Count.Text = "Новый товар";
                }
                else
                {
                    var storage = context.Storages.FirstOrDefault(s => s.ProductId == product.ProductId);
                    if (storage != null)
                    {
                        Prod_Count.Text = $"Количество товара на складе: {storage.QuantityOfProducts.ToString()}";
                    }
                    else
                    {
                        Prod_Count.Text = "0";
                    }
                }
            }
        }
    }
}
   