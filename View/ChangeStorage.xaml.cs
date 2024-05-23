using Microsoft.EntityFrameworkCore;
using Shop.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Shop.View
{
    public partial class ChangeStorage : Window
    {
        private StorageWindow _storageWindow;
        private Storage _currentStorage = new Storage();

        public delegate void DataChangedEventHandler(object sender, RoutedEventArgs e);
        public event DataChangedEventHandler DataChanged;

        public ChangeStorage(Storage selectedStorage)
        {
            InitializeComponent();
            _storageWindow = Application.Current.Windows.OfType<StorageWindow>().FirstOrDefault();
            if (selectedStorage != null)
                _currentStorage = selectedStorage;

            _currentStorage.DateDelivery = DateTime.Today;
            DateDeliveryTextBox.IsReadOnly = true;
            DataContext = _currentStorage;

            Closed += ChangeStorage_Closed; // Привязка к обработчику события Closed
        }

        private void ChangeStorage_Closed(object sender, EventArgs e)
        {
            _storageWindow.DataContext = _storageWindow.Storages;
        }




        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                var existingStorage = context.Storages.Find(_currentStorage.StorageId);
                if (existingStorage != null)
                {
                    var product = context.Products.FirstOrDefault(p => p.ProductId == int.Parse(ProductIdTextBox.Text));
                    if (product != null)
                    {
                        existingStorage.ProductId = product.ProductId;
                        existingStorage.QuantityOfProducts = int.Parse(QuantityOfProductsTextBox.Text);
                        existingStorage.TotalCost = product.PriceUnit * existingStorage.QuantityOfProducts;
                        existingStorage.DateDelivery = DateTime.Today;
                        existingStorage.Product = product;
                    }

                    DataContext = _currentStorage;

                    try
                    {
                        context.SaveChanges();
                        _storageWindow.Stor.ItemsSource = context.Storages.Include("Product").ToList(); // Установить источник данных таблицы
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                DataChanged?.Invoke(this, e);
                MessageBox.Show("Информация сохранена!", "Успешно");

                Close(); // Закрыть окно ChangeStorage
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
