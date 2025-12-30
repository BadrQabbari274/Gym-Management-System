using System.Windows;
using System.Windows.Controls;

namespace Forma_System
{
    public partial class InputBox : Window
    {
        public string PasswordText => passwordBox.Password;

        public InputBox(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}