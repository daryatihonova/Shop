using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using Shop.Model;

namespace Shop.View
{
    public partial class NewSeller : Window
    {
        public NewSeller()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Seller newSeller = new Seller
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Patronymic = PatronymicTextBox.Text,
                Login = LoginTextBox.Text,
                Password = PasswordTextBox.Text 
            };

            using (var context = new ShopContext())
            {
                context.Sellers.Add(newSeller);
                context.SaveChanges();
            }

            MessageBox.Show("Продавец успешно добавлен в базу данных!", "Успешно");
        }


        private void CancelButton_Click(Object sender, RoutedEventArgs e)
        {
            ClearTextBox();
            SellerWindow sellerWindow = new SellerWindow();
            this.Hide();
            sellerWindow.Show();
        }
        private void ClearTextBox()
        {
            FirstNameTextBox.Text = " ";
            LastNameTextBox.Text = " ";
            PatronymicTextBox.Text = " ";
            LoginTextBox.Text = " ";
            PasswordTextBox.Text = " ";
        }

    }
}
