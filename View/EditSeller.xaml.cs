using Microsoft.EntityFrameworkCore;
using Shop.Model;
using System;
using System.Windows;

namespace Shop.View
{
    public partial class EditSeller : Window
    {
        private Seller _selectedSeller;

        public EditSeller(Seller selectedSeller)
        {
            _selectedSeller = selectedSeller;
            InitializeComponent();

            // Initialize TextBoxes with data from the selected seller
            FirstNameTextBox.Text = _selectedSeller.FirstName;
            LastNameTextBox.Text = _selectedSeller.LastName;
            PatronymicTextBox.Text = _selectedSeller.Patronymic;
            LoginTextBox.Text = _selectedSeller.Login;
            PasswordTextBox.Text = _selectedSeller.Password;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                try
                {
                    // Update the seller's properties with the data from the TextBoxes
                    _selectedSeller.FirstName = FirstNameTextBox.Text;
                    _selectedSeller.LastName = LastNameTextBox.Text;
                    _selectedSeller.Patronymic = PatronymicTextBox.Text;
                    _selectedSeller.Login = LoginTextBox.Text;
                    _selectedSeller.Password = PasswordTextBox.Text;

                    // Attach the modified seller to the context
                    context.Attach(_selectedSeller);

                    // Mark the seller as modified
                    context.Entry(_selectedSeller).State = EntityState.Modified;

                    // Save the changes to the database
                    context.SaveChanges();

                    MessageBox.Show("Данные успешно сохранены.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения данных: {ex.Message}");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
