using System;
using System.Windows;
using Shop.Model;

namespace Shop.View
{
    public partial class NewSeller : Window
    {
        private Seller _currentSeller = new Seller();
       

        public NewSeller(Seller selectedSeller)
        {
            InitializeComponent();

            if (selectedSeller != null)
                _currentSeller = selectedSeller;

            DataContext = _currentSeller;
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                if (_currentSeller.SellerId == 0) // Новый продавец
                {
                    Seller newSeller = new Seller
                    {
                        FirstName = FirstNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        Patronymic = PatronymicTextBox.Text,
                        Login = LoginTextBox.Text,
                        Password = PasswordTextBox.Text
                    };

                    context.Sellers.Add(newSeller);
                }
                else // Существующий продавец
                {
                    var existingSeller = context.Sellers.Find(_currentSeller.SellerId);
                    if (existingSeller != null)
                    {
                        existingSeller.FirstName = FirstNameTextBox.Text;
                        existingSeller.LastName = LastNameTextBox.Text;
                        existingSeller.Patronymic = PatronymicTextBox.Text;
                        existingSeller.Login = LoginTextBox.Text;
                        existingSeller.Password = PasswordTextBox.Text;
                    }
                }

                context.SaveChanges();
            }

            MessageBox.Show("Информация сохранена!", "Успешно");
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }
    }
}
