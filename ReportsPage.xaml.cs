using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.ComponentModel;
using System.Windows.Data;

namespace Forma_System
{
    public partial class ReportsPage : Window
    {
        private readonly ReportsService _service = new ReportsService();
        private DateTime _from, _to;

        public ReportsPage()
        {
            InitializeComponent();

            // Default filter
            RbToday.IsChecked = true;
            RbToday.Checked += (s, e) => LoadReport();
            RbWeek.Checked += (s, e) => LoadReport();
            RbMonth.Checked += (s, e) => LoadReport();
            Btn_ShowReport.Click += (s, e) => LoadReport();

            TxtSearchAttendance.TextChanged += (s, e) => LoadReport();
            TxtSearchSubs.TextChanged += (s, e) => LoadReport();


            LoadReport();
        }

        private void LoadReport()
        {
            // Determine period
            if (RbToday.IsChecked == true)
                (_from, _to) = _service.GetPeriod(PeriodType.Today);
            else if (RbWeek.IsChecked == true)
                (_from, _to) = _service.GetPeriod(PeriodType.ThisWeek);
            else
                (_from, _to) = _service.GetPeriod(PeriodType.ThisMonth);

            // KPIs
            TbAttendanceCount.Text = _service.GetAttendanceCount(_from, _to).ToString();
            TbExpiringCount.Text = _service.GetExpiringSubscriptions(7).ToString();

            // Grids
            GridAttendance.ItemsSource = _service.GetAttendanceDetails(_from, _to, TxtSearchAttendance.Text);
            GridSubscriptions.ItemsSource = _service.GetSubscriptionDetails(TxtSearchSubs.Text);
        }
        private void Button_Return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"تقرير {now:yyyy-MM-dd_HH-mm-ss}.xlsx"

            };
            if (dlg.ShowDialog() != true) return;

            using (var wb = new XLWorkbook())
            {
                ExportWorksheet(wb, GridAttendance.ItemsSource, "الحضور");
                ExportWorksheet(wb, GridSubscriptions.ItemsSource, "الاشتراكات");
                wb.SaveAs(dlg.FileName);
            }

            MessageBox.Show("تم تصدير التقرير إلى Excel.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"تقرير {now:yyyy-MM-dd_HH-mm-ss}.pdf"

            };
            if (dlg.ShowDialog() != true) return;

            using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
            {
                var doc = new Document(PageSize.A4.Rotate(), 10, 10, 20, 20);
                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                AddPdfTable(doc, "حضور", GridAttendance);
                doc.Add(new iTextSharp.text.Paragraph("\n"));
                AddPdfTable(doc, "اشتراكات", GridSubscriptions);

                doc.Close();
            }

            MessageBox.Show("تم تصدير التقرير إلى PDF.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // === Methods to avoid "does not exist in the current context" ===

        private void ExportWorksheet(XLWorkbook wb, System.Collections.IEnumerable data, string sheetName)
        {
            var ws = wb.Worksheets.Add(sheetName);
            var list = data.Cast<object>().ToList();
            if (!list.Any()) return;

            var props = list.First().GetType().GetProperties();
            // Header
            for (int i = 0; i < props.Length; i++)
                ws.Cell(1, i + 1).Value = props[i].Name;

            // Data
            int row = 2;
            foreach (var item in list)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    var val = props[i].GetValue(item);
                    ws.Cell(row, i + 1).Value = val?.ToString() ?? "";
                }
                row++;
            }

            ws.Columns().AdjustToContents();
        }
        private void AddPdfTable(Document doc, string title, DataGrid grid)
        {
            if (grid == null || grid.Items.Count == 0) return;

            doc.Add(new Paragraph(title, FontFactory.GetFont("Arial", 16, BaseColor.BLACK)));
            doc.Add(new Paragraph("\n"));

            var columns = grid.Columns
                             .OfType<DataGridTextColumn>()
                             .ToList();

            var table = new PdfPTable(columns.Count) { WidthPercentage = 100 };

            // رؤوس الأعمدة
            foreach (var column in columns)
            {
                string header = column.Header?.ToString() ?? "";
                table.AddCell(new PdfPCell(new Phrase(header, FontFactory.GetFont("Arial", 12, BaseColor.WHITE)))
                {
                    BackgroundColor = BaseColor.GRAY,
                    Padding = 5
                });
            }

            // بيانات الصفوف
            foreach (var rawItem in grid.Items)
            {
                // نتجاهل الـ placeholder الخاص بصف الإضافة الجديد:
                if (rawItem == CollectionView.NewItemPlaceholder)
                    continue;

                foreach (var column in columns)
                {
                    // نستخرج TextBlock من الخلية
                    var cellContent = column.GetCellContent(rawItem) as TextBlock;
                    string text = cellContent?.Text ?? "";

                    table.AddCell(new PdfPCell(new Phrase(text, FontFactory.GetFont("Arial", 10)))
                    {
                        Padding = 4
                    });
                }
            }

            doc.Add(table);
        }




        // ==== ترجمة أسماء الأعمدة للعربي (متوافقة مع C# 7.3) ====
        private string GetHeaderText(string propertyName)
        {
            if (propertyName == "Date")
                return "التاريخ";
            else if (propertyName == "Customer")
                return "العميل";
            else if (propertyName == "Notes")
                return "ملاحظات";
            else if (propertyName == "SubscribeDate")
                return "تاريخ الاشتراك";
            else if (propertyName == "Package")
                return "الباقة";
            else if (propertyName == "Status")
                return "الحالة";
            else
                return propertyName; // fallback
        }

    }
}
