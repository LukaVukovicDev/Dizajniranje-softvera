using CoworkingApp.BusinessLogic.Database;
using CoworkingApp.BusinessLogic.Models;
using CoworkingApp.BusinessLogic.Services;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ICollectionView usersView;
        private List<User> allUsers = new List<User>();
        public MainWindow()
        {
            InitializeComponent();

            // Kreiraj i dodeli ViewModel
            var config = new ConfigReader("config.txt"); // uzimamo chain name iz fajla
            var facade = CoworkingFacade.GetInstance(config.ConnectionString);
            DataContext = new MainWindowViewModel(facade, config);

            // Sada je DataContext sigurno postavljen
            usersView = CollectionViewSource.GetDefaultView(
                ((MainWindowViewModel)DataContext).Users
            );
            usersView.Filter = FilterUsers;
        }

        private bool isEditingUsers = false;

        private void EditUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isEditingUsers)
            {
                // Omoguci editovanje DataGrid-a
                UsersDataGrid.IsReadOnly = false;

                // Promeni tekst dugmeta u "Save"
                EditUserBtn.Content = "Save";

                isEditingUsers = true;
            }
            else
            {
                // Sacuvaj promene 
                UsersDataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                UsersDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                // Ponovo zakljuci DataGrid
                UsersDataGrid.IsReadOnly = true;

                // Vrati dugme na "Edit User"
                EditUserBtn.Content = "Edit User";

                isEditingUsers = false;
            }
        }

        private bool FilterUsers(object obj)
        {
            if (SearchBox == null)
                return true;
            if (obj is not User user)
                return false;

            // SEARCH
            string searchText = SearchBox.Text?.ToLower() ?? "";

            if (!string.IsNullOrEmpty(searchText))
            {
                if (!user.FullName.ToLower().Contains(searchText) &&
                    !user.Email.ToLower().Contains(searchText))
                    return false;
            }

            // MEMBERSHIP
            string membership = "All";

            if (MembershipFilter.SelectedItem is ComboBoxItem m)
                membership = m.Content?.ToString() ?? "All";

            if (membership != "All")
{
    if (user.MembershipType == null || user.MembershipType.Name != membership)
        return false;
}

            // STATUS
            string status = "All";

            if (StatusFilter.SelectedItem is ComboBoxItem s)
                status = s.Content?.ToString() ?? "All";

            if (status != "All")
            {
                if (user.Status.ToString() != status)
                    return false;
            }

            return true;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (usersView != null)
                usersView.Refresh();

        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (usersView != null)
                usersView.Refresh();

            
        }

        public void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();

            this.Close();
        }
    }


}