using Microsoft.EntityFrameworkCore;
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
            string Login = Login_TextBox.Text;
            string Password = Password_TextBox.Text;

            if (Login == "admin" && Password == "admin")
            {
                MainAdminWindow mainAdminWindow = new MainAdminWindow();
                this.Hide();
                mainAdminWindow.Show();
            }
            else
            {
                SaleWindowForSeller saleWindowForSeller = new SaleWindowForSeller();
                this.Hide();
                saleWindowForSeller.Show();
            }
        }
    }
}

