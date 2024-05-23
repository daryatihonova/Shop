using Shop.Model;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для NewSaleWindow.xaml
    /// </summary>
    public partial class NewSaleWindow : Window
    {

        private Sale _currentSale = new Sale();

        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        public NewSaleWindow(Sale selectedSale)
        {
            InitializeComponent();
            if (selectedSale != null)
                _currentSale = selectedSale;

            DataContext = _currentSale;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                if (_currentSale.SaleId == 0) 
                {
                    Sale newSale = new Sale
                    {
                        ProductId = int.Parse(ProductIdTextBox.Text),
                        AmountOfProducts = int.Parse(AmountOfProductsTextBox.Text),
                        Cost = decimal.Parse(CostTextBox.Text),
                        Date = DateTime.Parse(DateTextBox.Text),
                        SellerId = int.Parse(SellerIdTextBox.Text)
                    };

                    context.Sales.Add(newSale);
                }
                else 
                {
                    var existingSale = context.Sales.Find(_currentSale.SaleId);
                    if (existingSale != null)
                    {
                        existingSale.ProductId = ProductIdTextBox.Text;
                        existingSale.AmountOfProducts = AmountOfProductsTextBox.Text;
                        existingSale.Cost = CostTextBox.Text;
                        existingSale.Date = DateTextBox.Text;
                        existingSale.SellerId = SellerIdTextBox.Text;
                    }
                }

                context.SaveChanges();
                DataContext = _currentSale;
               
                DataChanged?.Invoke(this, EventArgs.Empty);


            }

            MessageBox.Show("Информация сохранена!", "Успешно");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }









    }

}
