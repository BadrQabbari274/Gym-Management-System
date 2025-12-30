// UsersPage.xaml.cs using System; using System.Linq; using System.Windows; using System.Windows.Controls;

using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System;

namespace Forma_System
{
    public partial class UsersPage : Window
    {
        private DataBaseForma2025Entities db = new DataBaseForma2025Entities(); private int selectedId = 0;

        public UsersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            DG.ItemsSource = db.Users.ToList();
            TextBox_UserId.Clear();
            TextBox_UserName.Clear();
            TextBox_UserPassword.Clear();
            TextBox_UserNotes.Clear();
            TextBox_UsersSearsh.Clear();
            selectedId = 0;
        }

        private void Button_Return_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_UserName.Text) || string.IsNullOrWhiteSpace(TextBox_UserPassword.Text))
            {
                MessageBox.Show("من فضلك ادخل الاسم وكلمة المرور.", "ملحوظة", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                var user = new User
                {
                    Name = TextBox_UserName.Text,
                    Password = TextBox_UserPassword.Text,
                    Notes = TextBox_UserNotes.Text
                };
                db.Users.Add(user);
                db.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG.SelectedItem is User usr)
            {
                selectedId = usr.Id;
                TextBox_UserId.Text = usr.Id.ToString();
                TextBox_UserName.Text = usr.Name;
                TextBox_UserPassword.Text = usr.Password;
                TextBox_UserNotes.Text = usr.Notes;
            }
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            if (selectedId <= 0)
            {
                MessageBox.Show("من فضلك اختر مستخدم للتعديل.", "ملحوظة", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                var usr = db.Users.Find(selectedId);
                if (usr != null)
                {
                    if (usr.Name == "Admin")
                    {
                        if (TextBox_UserName.Text != "Admin")
                        {
                            MessageBox.Show("لا يمكنك تعديل اسم الادمن لكن يمكنك تغيير كلمة المرور.", "ملحوظة", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        usr.Name = "Admin";
                    }
                    else
                    {
                        usr.Name = TextBox_UserName.Text;
                    }
                    usr.Password = TextBox_UserPassword.Text;
                    usr.Notes = TextBox_UserNotes.Text;
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
            if (selectedId <= 0)
            {
                MessageBox.Show("من فضلك اختر مستخدم للحذف.", "ملحوظة", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var usr = db.Users.Find(selectedId);
            if (usr != null)
            {
                db.Users.Remove(usr);
                db.SaveChanges();
                LoadData();
            }
        }

        private void TextBox_UsersSearsh_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = TextBox_UsersSearsh.Text.ToLower();
            DG.ItemsSource = db.Users
                .Where(u => u.Name.ToLower().Contains(keyword))
                .ToList();
        }

    }

}