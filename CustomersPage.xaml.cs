using System;
using System.Linq;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace Forma_System
{
    public partial class CustomersPage : Window
    {
        private DataBaseForma2025Entities db = new DataBaseForma2025Entities();
        private int selectedId = 0;

        public CustomersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // تجلب كل العملاء من قاعدة البيانات
            DG.ItemsSource = db.Customers.ToList();

            // إعادة تهيئة الحقول...
            TextBox_CustomerSearsh.Clear();
            TextBox_CustomerCode.Clear();
            TextBox_CustomerName.Clear();
            TextBox_CustomerNotes.Clear();
            selectedId = 0;
        }


        private void Button_Return_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_CustomerCode.Text) ||
                string.IsNullOrWhiteSpace(TextBox_CustomerName.Text))
            {
                MessageBox.Show("من فضلك ادخل الاسم و الكود.", "ملحوظة",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var customer = new Customer
                {
                    CustomerCode = TextBox_CustomerCode.Text,
                    Name = TextBox_CustomerName.Text,
                    Notes = TextBox_CustomerNotes.Text
                };
                db.Customers.Add(customer);
                db.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG.SelectedItem is Customer cust)
            {
                selectedId = cust.Id;
                TextBox_CustomerId.Text = cust.Id.ToString();
                TextBox_CustomerCode.Text = cust.CustomerCode;
                TextBox_CustomerName.Text = cust.Name;
                TextBox_CustomerNotes.Text = cust.Notes;
            }
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            if (selectedId <= 0)
            {
                MessageBox.Show("من فضلك اختر العميل للتعديل.", "ملحوظة",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var customer = db.Customers.Find(selectedId);
                if (customer != null)
                {
                    customer.CustomerCode = TextBox_CustomerCode.Text;
                    customer.Name = TextBox_CustomerName.Text;
                    customer.Notes = TextBox_CustomerNotes.Text;
                    db.SaveChanges();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedId <= 0)
            {
                MessageBox.Show("من فضلك اختر العميل للحذف", "ملحوظة",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                var customer = db.Customers.Find(selectedId);
                if (customer != null)
                {
                    db.Customers.Remove(customer);
                    db.SaveChanges();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_CustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = TextBox_CustomerSearsh.Text.ToLower();
            DG.ItemsSource = db.Customers
                .Where(c => c.CustomerCode.ToLower().Contains(keyword)
                         || c.Name.ToLower().Contains(keyword))
                .ToList();
        }
    }
}