using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Forma_System
{
    public partial class SubscribePage : Window
    {
        DataBaseForma2025Entities db = new DataBaseForma2025Entities();

        public SubscribePage()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            DG.ItemsSource = db.Subscribes
            .Include(s => s.Customer)
            .Include(s => s.Baqat)
            .ToList();

            // تحميل العملاء في ComboBox
            ComboBox_Customers.ItemsSource = db.Customers.ToList();
            ComboBox_Customers.DisplayMemberPath = "Name";         // عرض الاسم
            ComboBox_Customers.SelectedValuePath = "Id";           // اختيار بالـ Id
            ComboBox_Customers.IsTextSearchEnabled = true;

            // تحميل الباقات في ComboBox
            ComboBox_Baqa.ItemsSource = db.Baqats.ToList();
            ComboBox_Baqa.DisplayMemberPath = "Name";              // عرض الاسم
            ComboBox_Baqa.SelectedValuePath = "Id";                // اختيار بالـ Id
            ComboBox_Baqa.IsTextSearchEnabled = true;
        }

        private void Button_Return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_Customers.SelectedValue == null || ComboBox_Baqa.SelectedValue == null)
            {
                MessageBox.Show("من فضلك اختر المشترك و اسم الباقة *_*", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var subscribe = new Subscribe
                {
                    CustomerId = (int)ComboBox_Customers.SelectedValue,
                    BaqaId = (int)ComboBox_Baqa.SelectedValue ,
                    SubscribeDate = DateTime.Now
                };

                db.Subscribes.Add(subscribe);
                db.SaveChanges();

                // تنظيف الواجهة
                TextBox_SubscribeId.Text = string.Empty;
                ComboBox_Customers.SelectedItem = null;
                ComboBox_Baqa.SelectedItem = null;

                Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public int DGSelectedId;

        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG.SelectedItem is Subscribe sub)
            {
                DGSelectedId = sub.Id;
                TextBox_SubscribeId.Text = sub.Id.ToString();

                // ضع الـ SelectedValue وليس DisplayMemberPath أو SelectedValuePath
                ComboBox_Customers.SelectedValue = sub.CustomerId;
                ComboBox_Baqa.SelectedValue = sub.BaqaId;
            }
        }


        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            if (DGSelectedId <= 0)
            {
                MessageBox.Show("من فضلك اختر اشتراك للتعديل", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var sub = db.Subscribes.Find(DGSelectedId);
                if (sub != null)
                {
                    if (ComboBox_Customers.SelectedValue != null && ComboBox_Baqa.SelectedValue != null)
                    {
                        sub.CustomerId = (int)ComboBox_Customers.SelectedValue;
                        sub.BaqaId = (int)ComboBox_Baqa.SelectedValue;
                        db.SaveChanges();

                        // تنظيف
                        TextBox_SubscribeId.Text = string.Empty;
                        ComboBox_Customers.SelectedItem = null;
                        ComboBox_Baqa.SelectedItem = null;
                        DGSelectedId = 0;

                        Load();
                    }
                    else
                    {
                        MessageBox.Show("من فضلك اختر الباقة والعميل قبل التحديث.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DGSelectedId <= 0)
            {
                MessageBox.Show("من فضلك اختر اشتراك للحذف", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var sub = db.Subscribes.Find(DGSelectedId);
            if (sub != null)
            {
                db.Subscribes.Remove(sub);
                db.SaveChanges();

                // تنظيف
                TextBox_SubscribeId.Text = string.Empty;
                ComboBox_Customers.SelectedItem = null;
                ComboBox_Baqa.SelectedItem = null;
                DGSelectedId = 0;

                Load();
            }
        }

        private void TextBox_SubscribeSearsh_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = TextBox_SubscribeSearsh.Text.ToLower();
            DG.ItemsSource = db.Subscribes
                .Where(s => s.Customer.Name.ToLower().Contains(keyword) || s.Baqat.Name.ToLower().Contains(keyword))
                .ToList();
        }
    }
}