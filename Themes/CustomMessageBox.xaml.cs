using System.Windows;
using System.Windows.Input;

namespace Forma_System
{
    public partial class CustomMessageBox : Window
    {
        public string Message { get; }
        public ICommand OKCommand { get; }

        public CustomMessageBox(string message)
        {
            InitializeComponent();
            Message = message;

            OKCommand = new RelayCommand(_ => this.Close());
            this.DataContext = this;
        }

        // طريقة عرض سهلة
        public static void Show(string message)
        {
            var dlg = new CustomMessageBox(message);
            dlg.ShowDialog();
        }
    }
}
