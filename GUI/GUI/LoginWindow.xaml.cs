using System.Windows;
using System.Windows.Input;

namespace GUI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
           // this.DataContext = ConfigManager.Instance;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnLogin_Click(sender, e);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPass.Password;

            if (username == "admin" && password == "admin")
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}