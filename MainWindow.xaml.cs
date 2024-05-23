using Microsoft.EntityFrameworkCore;
using Shop.Model;
using Shop.View;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Admin_Menu(object sender, EventArgs e)
        {
            using (var context = new ShopContext())
            {
                string login = Login_TextBox.Text;
                string password = Password_TextBox.Text;

                if (login == "admin" && password == "admin")
                {
                    MainAdminWindow mainAdminWindow = new MainAdminWindow();
                    this.Hide();
                    mainAdminWindow.Show();
                }
                else
                {
                    var seller = context.Sellers.FirstOrDefault(s => s.Login == login && s.Password == password);

                    if (seller != null)
                    {
                        SaleWindowForSeller saleWindowForSeller = new SaleWindowForSeller();
                        this.Hide();
                        saleWindowForSeller.Show();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}

