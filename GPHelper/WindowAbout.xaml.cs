using System.Windows;

namespace GPHelper
{
    public partial class WindowAbout : Window
    {
        public WindowAbout()
        {
            InitializeComponent();
        }

        private void CloseAboutWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
