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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Forma_System
{
    /// <summary>
    /// Interaction logic for HomaPage.xaml
    /// </summary>
    public partial class HomaPage : Window
    {
        DataBaseForma2025Entities db = new DataBaseForma2025Entities();
        private DispatcherTimer _timer;

        private SubscribePage _subscribePage;
        private AttendedsPage _attendedsPage;
        private CustomersPage _customersPage;
        private BaqatPage _baqatPage;
        private AboutUsPage _aboutUsPage;
        private UsersPage _usersPage;
        private ReportsPage _reportsPage;
        private MainWindow _mainWindow;
        public HomaPage(string CurrentUser)
        {
            InitializeComponent();
            TextBlock_Identity.Text = CurrentUser;
            StartUp();
            StartClock();
        }
        // 1️⃣ دالة لإظهار مربّع الحوار ورجوع النتيجة
        private bool ShowConfirmDialog()
        {
            var result = MessageBox.Show(
                "هل أنت متأكد أنك تريد الخروج؟",
                "تأكيد الخروج",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }
        private bool allowClose = false;
        // 2️⃣ نعيد كتابة OnClosing بحيث يعتمد على قيمة الدالة أعلاه
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!allowClose)
            {
                // لو رجعت false (يعني المستخدم اختار No) نمنع الإغلاق
                if (!ShowConfirmDialog())
                {
                    e.Cancel = true;
                    return;
                }

                // غير كده نترك الإغلاق يتمّ عادي
                base.OnClosing(e);
                Application.Current.Shutdown();
            }
        }

        // 3️⃣ في زر الإرجاع نكتفي باستدعاء Close() فقط
        private void Button_Return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartUp()
        {
            TextBlock_date.Text = DateTime.Now.ToString("G");
        }

        private void StartClock()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                var now = DateTime.Now;
                TextBlock_date.Text = now.ToString("G");
            };
            _timer.Start();
        }
        private void Button_Subscribe_Click(object sender, RoutedEventArgs e)
        {
            if (_subscribePage == null || !_subscribePage.IsLoaded)
            {
                _subscribePage = new SubscribePage();
                _subscribePage.Closed += (s, a) => _subscribePage = null;
                _subscribePage.Show();
            }
            else
            {
                _subscribePage.Activate();
            }
        }

        private void Button_Attendeds_Click(object sender, RoutedEventArgs e)
        {
            if (_attendedsPage == null || !_attendedsPage.IsLoaded)
            {
                _attendedsPage = new AttendedsPage();
                _attendedsPage.Closed += (s, a) => _attendedsPage = null;
                _attendedsPage.Show();
            }
            else
            {
                _attendedsPage.Activate();
            }
        }

        private void Button_Customers_Click(object sender, RoutedEventArgs e)
        {
            if (_customersPage == null || !_customersPage.IsLoaded)
            {
                _customersPage = new CustomersPage();
                _customersPage.Closed += (s, a) => _customersPage = null;
                _customersPage.Show();
            }
            else
            {
                _customersPage.Activate();
            }
        }

        private void Button_Baqat_Click(object sender, RoutedEventArgs e)
        {
            if (_baqatPage == null || !_baqatPage.IsLoaded)
            {
                _baqatPage = new BaqatPage();
                _baqatPage.Closed += (s, a) => _baqatPage = null;
                _baqatPage.Show();
            }
            else
            {
                _baqatPage.Activate();
            }
        }

        private void Button_AboutUs_Click(object sender, RoutedEventArgs e)
        {
            if (_aboutUsPage == null || !_aboutUsPage.IsLoaded)
            {
                _aboutUsPage = new AboutUsPage();
                _aboutUsPage.Closed += (s, a) => _aboutUsPage = null;
                _aboutUsPage.Show();
            }
            else
            {
                _aboutUsPage.Activate();
            }
        }

        private void Button_Users_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlock_Identity.Text != "Admin")
            {
                MessageBox.Show("ليس لديك صلاحية الادمن للوصول لهذه الصفحة", "", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (_usersPage == null || !_usersPage.IsLoaded)
            {
                _usersPage = new UsersPage();
                _usersPage.Closed += (s, a) => _usersPage = null;
                _usersPage.Show();
            }
            else
            {
                _usersPage.Activate();
            }
        }

        private void Button_Reports_Click(object sender, RoutedEventArgs e)
        {
            if (_reportsPage == null || !_reportsPage.IsLoaded)
            {
                _reportsPage = new ReportsPage();
                _reportsPage.Closed += (s, a) => _reportsPage = null;
                _reportsPage.Show();
            }
            else
            {
                _reportsPage.Activate();
            }
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null || !_mainWindow.IsLoaded)
            {
                _mainWindow = new MainWindow();
                _mainWindow.Closed += (s, a) => _mainWindow = null;
                _mainWindow.Show();
                allowClose = true; // نسمح بالإغلاق
                this.Close();
            }
            else
            {
                _mainWindow.Activate();
            }
        }
    }
}
