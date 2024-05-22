using System;
using System.Linq;
using System.Windows;
using Shop.Model;

namespace Shop.View
{
    public partial class NewSeller : Window
    {
        private Seller _currentSeller = new Seller();
        
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;


        public NewSeller(Seller selectedSeller)
        {
            InitializeComponent();

            if (selectedSeller != null)
                _currentSeller = selectedSeller;

            DataContext = _currentSeller;
            PasswordTextBox.LostFocus += PasswordTextBox_LostFocus; // Добавляем обработчик события LostFocus для поля пароля 

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
                DataContext = _currentSeller;
                // Оповещаем SellerWindow о том, что данные были изменены
                DataChanged?.Invoke(this, EventArgs.Empty);


            }

            MessageBox.Show("Информация сохранена!", "Успешно");
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string password = PasswordTextBox.Text;

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов.");
            }
            else if (!password.Any(char.IsUpper))
            {
                MessageBox.Show("Пароль должен содержать минимум 1 прописную букву.");
            }
            else if (!password.Any(char.IsDigit))
            {
                MessageBox.Show("Пароль должен содержать минимум 1 цифру.");
            }
            else if (!password.Any(c => "!@#$%^".Contains(c)))
            {
                MessageBox.Show("Пароль должен содержать минимум один из следующих символов: ! @ # $ % ^.");
            }
        }

    }
}
