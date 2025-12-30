using Forma_System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Forma_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBaseForma2025Entities db = new DataBaseForma2025Entities();
        public MainWindow()
        {
            InitializeComponent();
            Checks();
            LoadDate();
            this.PreviewKeyDown += Window_PreviewKeyDown;
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();

            }
        }
        private void LoadDate()
        {
            try
            {
                ComboBox_UsersNames.ItemsSource = db.Users.ToList();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error {ex.Message}");
            }
        }
        private void Button_login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(PasswordBox_passworduser.Password) || ComboBox_UsersNames.SelectedValue == null)
                {
                    MessageBox.Show("اختر المستخدم وكلمة المرور", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string selectedUser = ComboBox_UsersNames.SelectedValue.ToString();
                string enteredPassword = PasswordBox_passworduser.Password;

                var login = db.Users.FirstOrDefault(l => l.Name == selectedUser && l.Password == enteredPassword);
                if (login != null)
                {
                    PasswordBox_passworduser.Clear();

                    // اغلق أي صفحات HomePage مفتوحة من جلسات قديمة
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is HomaPage)
                        {
                            window.Close();
                        }
                    }

                    // افتح الصفحة الجديدة للمستخدم الحالي
                    HomaPage home = new HomaPage(selectedUser);
                    home.Show();

                    this.Close(); // اغلق نافذة اللوجين
                }
                else
                {
                    MessageBox.Show("كلمة المرور غير صحيحة ):", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    PasswordBox_passworduser.Clear();
                    PasswordBox_passworduser.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void Dev()
        {
            MessageBox.Show("رقم هذا الجهاز غير مصرح به لكن وضع المطور مفعل", "ادخل كلمة مرور وضع المطور", MessageBoxButton.OK, MessageBoxImage.Information);

            if (!AskDeveloperPassword())
            {
                MessageBox.Show("كلمة المرور غير صحيحة. سيتم إغلاق البرنامج.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            MessageBox.Show("تم التحقق من وضع المطوّر بنجاح ✅", "أهلا بشمهندس بدر", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Checks()
        {
            try
            {

                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\DataBaseForma2025.mdf");

                if (!System.IO.File.Exists(path))
                {
                    MessageBox.Show("❌ قاعدة البيانات ليست في: \n" + path, "خطأ");
                    return;
                }
                string myPId = GetIDs.GetProcessorId();
                string myHDDId = GetIDs.GetWindowsDriveSerialNumber();
                string allowPId = "178BFBFF00600F20";
                string allowHDDId = "CVDA340004FK1802GN";

                string devKeyPath = AppDomain.CurrentDomain.BaseDirectory + "dev.bat";
                bool isDev = System.IO.File.Exists(devKeyPath);

                if (myPId == allowPId || myHDDId == allowHDDId)
                {
                    if (!isDev)
                    {
                        AntiReverseTools.StartProtection();
                    }

                }
                else
                {
                    if (!isDev)
                    {
                        MessageBox.Show("هذا الجهاز غير مصرح له باستخدام البرنامج.", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning);
                        Application.Current.Shutdown();
                    }
                    if (isDev)
                    {
                        Dev();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erorr: {ex.Message}");
            }
        }
        private bool AskDeveloperPassword()
        {
            string password = ShowPasswordDialog("من فضلك أدخل كلمة مرور المطوّر:");

            const string devPassword = "mysecret123";

            return password == devPassword;
        }

        private string ShowPasswordDialog(string message)
        {
            // (بدون تصميم فورم إضافي)
            var inputDialog = new InputBox(message);
            bool? result = inputDialog.ShowDialog();

            if (result == true)
                return inputDialog.PasswordText;

            return null;
        }


    }
}