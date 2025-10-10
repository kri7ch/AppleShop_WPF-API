using AppleShopWPF.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppleShopWPF.Pages
{
    public partial class AuthorizationPage : Page
    {
        private readonly ApiClient _apiClient;

        public AuthorizationPage()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
        }

        private async void btnEnterClick(object sender, RoutedEventArgs e)
        {
            string email = btnEmailEnter.Text.Trim();
            string password = (cbShowPassword.IsChecked == true) ? tbPasswordVisible.Text : btnPasswordEnter.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            bool success = await _apiClient.LoginAsync(email, password);

            if (success)
            {
                int userId = await _apiClient.GetUserIdByEmailAsync(email);
                if (userId > 0) AppState.CurrentUserId = userId;

                if (Services.AppState.IsAdmin)
                {
                    var window = Window.GetWindow(this);
                    window.Title = "Admin Panel";
                    NavigationService.Navigate(new AdminPage());
                }
                else
                {
                    var window = Window.GetWindow(this);
                    window.Title = "Menu";
                    NavigationService.Navigate(new MainMenuPage());
                }
            }
            
        }

        private void cbShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            tbPasswordVisible.Text = btnPasswordEnter.Password;
            tbPasswordVisible.Visibility = Visibility.Visible;
            btnPasswordEnter.Visibility = Visibility.Collapsed;
        }

        private void cbShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            btnPasswordEnter.Password = tbPasswordVisible.Text;
            btnPasswordEnter.Visibility = Visibility.Visible;
            tbPasswordVisible.Visibility = Visibility.Collapsed;
        }

        private void Entry_window()
        {
            var window = Window.GetWindow(this);
            window.Title = "Registration";
            NavigationService.Navigate(new RegistationPage());
        }

        private void hyperlinkRegistration(object sender, RoutedEventArgs e)
        {
            Entry_window();
        }
    }
}