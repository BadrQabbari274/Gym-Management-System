using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Forma_System
{ /// <summary> /// Interaction logic for BaqatPage.xaml /// </summary> 
    public partial class BaqatPage : Window
    {
        private DataBaseForma2025Entities db = new DataBaseForma2025Entities();

        private int selectedId = 0;

        public BaqatPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            DG.ItemsSource = db.Baqats.ToList();

            TextBox_BaqatId.Clear();
            TextBox_BaqatName.Clear();
            TextBox_BaqatNumOfDayM.Clear();
            TextBox_BaqatNumOfDayW.Clear();
            TextBox_BaqatNotes.Clear();
            TextBox_BaqatSearsh.Clear();
            selectedId = 0;
        }


        private void Button_Return_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_BaqatName.Text))
            {
                MessageBox.Show("من فضلك ادخل البيانات المطلوبة .", "ملحوظة", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var baqa = new Baqat
                {
                    Name = TextBox_BaqatName.Text,
                    DaysPerMonth = int.TryParse(TextBox_BaqatNumOfDayM.Text, out int dpm) ? dpm : 0,
                    MaxDaysPerWeek = int.TryParse(TextBox_BaqatNumOfDayW.Text, out int dpw) ? dpw : 0,
                    Notes = TextBox_BaqatNotes.Text
                };
                db.Baqats.Add(baqa);
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
            if (DG.SelectedItem is Baqat bq)
            {
                selectedId = bq.Id;
                TextBox_BaqatId.Text = bq.Id.ToString();
                TextBox_BaqatName.Text = bq.Name;
                TextBox_BaqatNumOfDayM.Text = bq.DaysPerMonth.ToString();
                TextBox_BaqatNumOfDayW.Text = bq.MaxDaysPerWeek.ToString();
                TextBox_BaqatNotes.Text = bq.Notes;
            }
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            if (selectedId <= 0)
            {
                MessageBox.Show("من فضلك اختر الباقة للتعديل", "ملحوظة", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var baqa = db.Baqats.Find(selectedId);
                if (baqa != null)
                {
                    baqa.Name = TextBox_BaqatName.Text;
                    baqa.DaysPerMonth = int.TryParse(TextBox_BaqatNumOfDayM.Text, out int dpm) ? dpm : baqa.DaysPerMonth;
                    baqa.MaxDaysPerWeek = int.TryParse(TextBox_BaqatNumOfDayW.Text, out int dpw) ? dpw : baqa.MaxDaysPerWeek;
                    baqa.Notes = TextBox_BaqatNotes.Text;
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
                MessageBox.Show("من فضلك اختر الباقة للحذف.", "ملحوظة", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var baqa = db.Baqats.Find(selectedId);
            if (baqa != null)
            {
                db.Baqats.Remove(baqa);
                db.SaveChanges();
                LoadData();
            }
        }

        private void TextBox_BaqatSearsh_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = TextBox_BaqatSearsh.Text.ToLower();
            DG.ItemsSource = db.Baqats
                .Where(b => b.Name.ToLower().Contains(keyword))
                .ToList();
        }

    }
}