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
    /// Логика взаимодействия для SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        private ObservableCollection<Sale> _sales;
        public ObservableCollection<Sale> Sales 
        {
            get { return _sales; }
            set { _sales = value; Sal.ItemsSource = value; }
        }
        public SaleWindow()
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
        private void back_admin_window(object sender, EventArgs e)
        {
            MainAdminWindow mainAdminWindow = new MainAdminWindow();
            this.Hide();
            mainAdminWindow.Show();
        }

        private void Click_Search(object sender, RoutedEventArgs e)
        {
            string date_Search = Date_Search.Text;
            bool isDateSearchEmpty = string.IsNullOrEmpty(date_Search);

            if (isDateSearchEmpty)
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
                        .AsQueryable();

                    if (!isDateSearchEmpty)
                    {
                        DateTime searchDate;
                        if (DateTime.TryParse(date_Search, out searchDate))
                        {
                            sales = sales.Where(sa => sa.Date.Date == searchDate.Date);
                        }
                        else
                        {
                            MessageBox.Show("Неверный формат даты. Введите дату в формате дд.ММ.гггг", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    var results = sales.ToList();

                    if (results.Count == 0)
                    {
                        MessageBox.Show("Продажи по указанным критериям не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Sales = new ObservableCollection<Sale>(results);
                    }
                }
            }
        }





        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Продажи и Склад за сегодня");
                worksheet.Columns().Width = 25;

                // Добавляем данные о продажах
                worksheet.Range("A1:F1").Merge().Value = "Отчет о продажах за день " + DateTime.Today.ToString("dd/MM/yyyy");
                worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                
                worksheet.Cell(2, 1).Value = "Наименование товара";
                worksheet.Cell(2, 2).Value = "Кол-во товаров";
                worksheet.Cell(2, 3).Value = "Стоимость";
                worksheet.Cell(2, 4).Value = "Дата";
                worksheet.Cell(2, 5).Value = "ФИО продавца";

                int rowSales = 3;
                foreach (var sale in Sales)
                {
                    if (sale.Date.Date == DateTime.Today)
                    {
                       
                        worksheet.Cell(rowSales, 1).Value = sale.Product.Name;
                        worksheet.Cell(rowSales, 2).Value = sale.AmountOfProducts;
                        worksheet.Cell(rowSales, 3).Value = sale.Cost;
                        worksheet.Cell(rowSales, 4).Value = sale.Date.ToString("dd/MM/yyyy");
                        worksheet.Cell(rowSales, 5).Value = sale.Seller.FullName;
                        rowSales++;
                    }
                }

                worksheet.Cell(2, 8).Value = "Выручка";
                worksheet.Cell(3, 8).FormulaA1 = $"SUM(C3:C{rowSales - 1})";
                worksheet.Cell("H2").Style.Font.Bold = true;

                // Добавляем данные о складе
                var storageWorksheet = workbook.Worksheets.Add("Склад");
                storageWorksheet.Columns().Width = 25;
                storageWorksheet.Range("A1:E1").Merge().Value = "Отчет о складе на " + DateTime.Today.ToString("dd/MM/yyyy");
                storageWorksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                
                storageWorksheet.Cell(2, 1).Value = "Наименование товара";
                storageWorksheet.Cell(2, 2).Value = "Кол-во товаров";
                storageWorksheet.Cell(2, 3).Value = "Общая стоимость";
                storageWorksheet.Cell(2, 4).Value = "Дата поставки";

                int rowStorage = 3;
                using (var dbContext = new ShopContext())
                {
                    var storages = dbContext.Storages.ToList();
                    foreach (var item in storages)
                    {
                        var product = dbContext.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                        string productName = product != null ? product.Name : "Н/Д";

                       
                        storageWorksheet.Cell(rowStorage, 1).Value = productName;
                        storageWorksheet.Cell(rowStorage, 2).Value = item.QuantityOfProducts;
                        storageWorksheet.Cell(rowStorage, 3).Value = item.TotalCost;
                        storageWorksheet.Cell(rowStorage, 4).Value = item.DateDelivery.ToString("dd/MM/yyyy");

                        rowStorage++;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "ОтчётЗаДень_" + DateTime.Today.ToString("yyyy-MM-dd") + ".xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx"; if (saveFileDialog.ShowDialog() == true)
                    { using (var fileStream = File.Create(saveFileDialog.FileName)) 
                        { stream.Seek(0, SeekOrigin.Begin); stream.CopyTo(fileStream); 
                        } MessageBox.Show("Отчет успешно экспортирован.", "Экспорт в Excel"); }
                }
            }
        }





    }
}
