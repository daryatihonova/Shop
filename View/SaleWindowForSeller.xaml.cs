using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Shop.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Threading;

namespace Shop.View
{
    /// <summary>
    /// Логика взаимодействия для SaleWindowForSeller.xaml
    /// </summary>
    public partial class SaleWindowForSeller : Window
    {
        private ObservableCollection<Sale> _sales;
        public ObservableCollection<Sale> Sales
        {
            get { return _sales; }
            set { _sales = value; Sal.ItemsSource = value; }
        }
        public SaleWindowForSeller()
        {
            InitializeComponent();
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();

            using (var context = new ShopContext())
            {
                Sales = new ObservableCollection<Sale>(context.Sales
                     .Include(s => s.Product)
                     .Include(s => s.Seller)
                     .ToList());
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        }

        private void back(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Hide();
            mainWindow.Show();
        }
        private void click_new_sale(object sender, RoutedEventArgs e)
        {
            NewSaleWindow newSaleWindow = new NewSaleWindow();
            newSaleWindow.DataChanged += UpdateDataGrid;
            newSaleWindow.Show();
        }



        private void Click_Prod_Search(object sender, RoutedEventArgs e)
        {
            string prodSearch = Prod_Search.Text;
            if (string.IsNullOrEmpty(prodSearch))
            {
                Sales = new ObservableCollection<Sale>(_sales);
            }
            else
            {
                using (var context = new ShopContext())
                {
                    var sales = context.Sales
                        .Include(s => s.Product)
                        .Include(s => s.Seller)
                        .Where(s => s.Product.Name.Contains(prodSearch))
                        .ToList();

                    if (sales.Count == 0)
                    {
                        MessageBox.Show("Продажа такого товара не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Sales = new ObservableCollection<Sale>(sales);
                    }
                }
            }
        }





        private void UpdateDataGrid(object sender, EventArgs e)
        {
            // Обновление данных в таблице
            using (var context = new ShopContext())
            {
                Sales = new ObservableCollection<Sale>(context.Sales
                    .Include(s => s.Product)
                    .Include(s => s.Seller)
                    .ToList());
            }
        }


        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Продажи за сегодня");

                
                worksheet.Columns().Width = 25;

                // Объединение ячеек с A1 до F1 и вставка текущей даты
                worksheet.Range("A1:F1").Merge().Value = "Отчет за день " + DateTime.Today.ToString("dd/MM/yyyy");
                worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Добавляем заголовки
                worksheet.Cell(2, 1).Value = "Код";
                worksheet.Cell(2, 2).Value = "Наименование товара";
                worksheet.Cell(2, 3).Value = "Кол-во товаров";
                worksheet.Cell(2, 4).Value = "Стоимость";
                worksheet.Cell(2, 5).Value = "Дата";
                worksheet.Cell(2, 6).Value = "ФИО продавца";

                // Добавляем данные о продажах
                int row = 3;
                foreach (var sale in Sales)
                {
                    if (sale.Date.Date == DateTime.Today)
                    {
                        worksheet.Cell(row, 1).Value = sale.SaleId;
                        worksheet.Cell(row, 2).Value = sale.Product.Name;
                        worksheet.Cell(row, 3).Value = sale.AmountOfProducts;
                        worksheet.Cell(row, 4).Value = sale.Cost;
                        worksheet.Cell(row, 5).Value = sale.Date.ToString("dd/MM/yyyy");
                        worksheet.Cell(row, 6).Value = sale.Seller.FullName;
                        row++;
                    }
                }

                // Добавляем заголовок и формулу для вычисления общей выручки за день
                worksheet.Cell(2, 8).Value = "Выручка";
                worksheet.Cell(3, 8).FormulaA1 = $"SUM(D3:D{row - 1})";
                worksheet.Cell("H2").Style.Font.Bold = true;

                // Сохраняем файл Excel
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "ОтчётЗаДень_" + DateTime.Today.ToString("yyyy-MM-dd") + ".xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        using (var fileStream = File.Create(saveFileDialog.FileName))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.CopyTo(fileStream);
                        }
                        MessageBox.Show("Отчет успешно экспортирован.", "Экспорт в Excel");
                    }
                }
            }
        }





    }
}
