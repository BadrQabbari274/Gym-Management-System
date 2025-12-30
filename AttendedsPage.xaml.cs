using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Forma_System
{
    public partial class AttendedsPage : Window
    {
        private DataBaseForma2025Entities db = new DataBaseForma2025Entities(); private int DGSelectedId = 0;

        public AttendedsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // تحميل البيانات مع الاسم
            DG.ItemsSource = db.Attendees
                              .Include(a => a.Customer)
                              .ToList();

            // تحميل العملاء
            ComboBox_Customers.ItemsSource = db.Customers.ToList();
            ComboBox_Customers.DisplayMemberPath = "Name";
            ComboBox_Customers.SelectedValuePath = "Id";
            ComboBox_Customers.IsTextSearchEnabled = true;
            ComboBox_Customers.SelectedIndex = -1;

            // تنظيف الملاحظات
            TextBox_AttendedNotes.Clear();
        }

        private void Button_Return_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_Customers.SelectedValue == null)
            {
                MessageBox.Show("من فضلك اختر العميل أولاً.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int customerId = (int)ComboBox_Customers.SelectedValue;
            DateTime now = DateTime.Now;

            if (!CheckBox_NoSubscribe.IsChecked.Value)
            {

                // 1. جيب أحدث اشتراك للعميل
                var lastSub = db.Subscribes
                    .Where(s => s.CustomerId == customerId)
                    .OrderByDescending(s => s.SubscribeDate)
                    .FirstOrDefault();

                string status;
                if (lastSub == null)
                {
                    status = "لا يوجد اشتراك";              // الحالة 1
                }
                else
                {
                    // 2. احسب عدد مرات الحضور في الأسبوع والشهر الحاليين
                    DateTime weekStart = now.AddDays(-((int)now.DayOfWeek + 6) % 7);
                    DateTime monthStart = new DateTime(now.Year, now.Month, 1);

                    int weekCount = db.Attendees
                        .Count(a => a.CustomerId == customerId && a.DateTime >= weekStart);
                    int monthCount = db.Attendees
                        .Count(a => a.CustomerId == customerId && a.DateTime >= monthStart);

                    // 3. احضر بيانات الباقة
                    var baqa = db.Baqats.Find(lastSub.BaqaId);

                    // 4. حدّد إذا منتهي أو لا
                    if (weekCount >= baqa.MaxDaysPerWeek || monthCount >= baqa.DaysPerMonth)
                        status = "منتهي الاشتراك";
                    else
                        status = "ساري";                    // الحالة 2
                }

                // 5. سجّل الحضور (أو عرض رسالة حالة)
                if (status == "ساري")
                {
                    var attend = new Attendee
                    {
                        CustomerId = customerId,
                        DateTime = now,
                        Notes = TextBox_AttendedNotes.Text
                    };
                    db.Attendees.Add(attend);
                    db.SaveChanges();
                    LoadData();
                }
                else
                {
                    MessageBox.Show($"لا يمكن تسجيل الحضور: {status}", "مرفوض", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                var attend = new Attendee
                {
                    CustomerId = customerId,
                    DateTime = now,
                    Notes = TextBox_AttendedNotes.Text
                };
                db.Attendees.Add(attend);
                db.SaveChanges();
                LoadData();
            }
        }


        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG.SelectedItem is Attendee attend)
            {
                DGSelectedId = attend.Id;
                TextBox_AttendedCustomerId.Text = attend.Id.ToString();
                ComboBox_Customers.SelectedValue = attend.CustomerId;
                TextBox_AttendedNotes.Text = attend.Notes;
            }
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            if (DGSelectedId <= 0)
            {
                MessageBox.Show("من فضلك اختر العميل للتعديل", "ملحوظة", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var record = db.Attendees.Find(DGSelectedId);
                if (record != null)
                {
                    if (ComboBox_Customers.SelectedValue != null)
                        record.CustomerId = (int)ComboBox_Customers.SelectedValue;

                    record.Notes = TextBox_AttendedNotes.Text;
                    db.SaveChanges();

                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DGSelectedId <= 0)
            {
                MessageBox.Show("من فضلك اختر العميل للحذف", "ملحوظة", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var record = db.Attendees.Find(DGSelectedId);
            if (record != null)
            {
                db.Attendees.Remove(record);
                db.SaveChanges();
                DGSelectedId = 0;
                LoadData();
            }
        }

    }

}