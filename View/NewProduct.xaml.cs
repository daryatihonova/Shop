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

namespace Shop.View
{
    /// <summary>
    /// Логика взаимодействия для NewProduct.xaml
    /// </summary>
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


            DataContext = _currentProduct;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new Model.ShopContext())
            {
                if (_currentProduct.ProductId == 0) // Добавление нового товара
                {
                    Model.Product newProduct = new Model.Product
                    {
                        Name = NameTextBox.Text,
                        Description = DescriptionTextBox.Text,
                        UnitOfMeasurement = UnitOfMeasurementTextBox.Text,
                        PriceUnit = decimal.Parse(PriceUnitTextBox.Text),
                        Quantity = int.Parse(QuantityTextBox.Text),
                        DateOfLastDelivery = DateTime.Parse(DateOfLastDeliveryTextBox.Text),
                    };

                    // Вычисление TotalCost для нового товара
                    decimal totalCost = newProduct.PriceUnit * newProduct.Quantity;

                    context.Products.Add(newProduct);

                    // Добавление записи на склад
                    Model.Storage newStorageEntry = new Model.Storage
                    {
                        QuantityOfProducts = newProduct.Quantity,
                        TotalCost = totalCost,
                        DateDelivery = DateTime.Now, // Дата совершения добавления
                        ProductId = newProduct.ProductId
                    };
                    context.Storages.Add(newStorageEntry);
                }
                else // Редактирование существующего товара
                {
                    var existingProduct = context.Products.Find(_currentProduct.ProductId);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = NameTextBox.Text;
                        existingProduct.Description = DescriptionTextBox.Text;
                        existingProduct.UnitOfMeasurement = UnitOfMeasurementTextBox.Text;
                        existingProduct.PriceUnit = decimal.Parse(PriceUnitTextBox.Text);
                        existingProduct.Quantity = int.Parse(QuantityTextBox.Text);
                        existingProduct.DateOfLastDelivery = DateTime.Parse(DateOfLastDeliveryTextBox.Text);

                        // Вычисление TotalCost для существующего товара
                        decimal totalCost = existingProduct.PriceUnit * existingProduct.Quantity;

                        // Обновление записи на складе
                        var storageEntry = context.Storages.FirstOrDefault(s => s.ProductId == existingProduct.ProductId);
                        if (storageEntry != null)
                        {
                            storageEntry.QuantityOfProducts = existingProduct.Quantity;
                            storageEntry.TotalCost = totalCost;
                            storageEntry.DateDelivery = DateTime.Now; // Дата совершения изменения
                        }
                    }
                }

                context.SaveChanges();
                DataContext = _currentProduct;
                DataChanged?.Invoke(this, EventArgs.Empty);
            }

            MessageBox.Show("Информация сохранена!", "Успешно");
        }



        private void CancelButton_Click(object sender, EventArgs e)
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
